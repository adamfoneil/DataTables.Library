using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;
using System.Collections.Generic;
using DataTables.Library;
using System.Data;
using Dapper;
using System.Threading.Tasks;
using System.Linq;

namespace Testing
{
    [TestClass]
    public class QueryTableTests
    {
        public const string DbName = "AdoUtil";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            LocalDb.TryDropDatabase(DbName, out _);
        }

        [TestMethod]
        public void SyncCallNoParams()
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {
                var dataTable = cn.QueryTable("SELECT * FROM [sys].[tables]");
            }
        }

        [TestMethod]
        public void SyncCallWithAnonObjParams()
        {
            const string objName = "sysfos";
            using (var cn = LocalDb.GetConnection(DbName))
            {
                var dataTable = cn.QueryTable("SELECT * FROM [sys].[objects] WHERE [name]=@name", new { name = objName });
                Assert.IsTrue(dataTable.Rows.Count == 1);
                Assert.IsTrue(dataTable.Rows[0].Field<string>("name").Equals(objName));
            }
        }
        
        [TestMethod]
        public void AsyncCallNoParams()
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {
                var dataTable = cn.QueryTableAsync("SELECT * FROM [sys].[objects]").Result;
                Assert.IsTrue(dataTable.Rows.Count > 0);
            }
        }

        [TestMethod]
        public void AsyncCallWithAnonObjParam()
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {
                var dataTable = cn.QueryTableAsync("SELECT * FROM [sys].[objects] WHERE [name]=@name", new { name = "sysfos" }).Result;
                Assert.IsTrue(dataTable.Rows.Count == 1);
            }
        }

        [TestMethod]
        public void SyncCallWithDictionaryParam()
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {
                // won't be any data, just want to make sure no exception using parameter
                var dataTable = cn.QueryTable("SELECT * FROM [sys].[tables] WHERE [name]=@name", new Dictionary<string, object>()
                {
                    { "name", "fred" }
                });
            }
        }

        [TestMethod]
        public void AsyncCallWithDictionaryParam()
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {                
                var dataTable = cn.QueryTableAsync("SELECT * FROM [sys].[objects] WHERE [name]=@name", new Dictionary<string, object>()
                {
                    { "name", "sysfos" }
                }).Result;
                Assert.IsTrue(dataTable.Rows.Count == 1);
            }

        }

        [TestMethod]
        public void QuerySchemaTable()
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {
                var schemaTable = cn.QuerySchemaTable("SELECT * FROM [sys].[tables]");
                Assert.IsTrue(schemaTable.Columns.Contains("ColumnName"));
            }
        }

        [TestMethod]
        public void QuerySchemaTableAsync()
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {
                var schemaTable = cn.QuerySchemaTableAsync("SELECT * FROM [sys].[tables]").Result;
                Assert.IsTrue(schemaTable.Columns.Contains("ColumnName"));
            }
        }

        [TestMethod]
        public void CreateTable()
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {                
                var createTable = cn.SqlCreateTableAsync("dbo", "SampleTable", "SELECT * FROM [sys].[tables]").Result;
                cn.Execute(createTable);
                cn.Execute("DROP TABLE [dbo].[SampleTable]");
            }
        }

        [TestMethod]
        public async Task SimpleTvpExample()
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {
                try
                {
                    cn.Execute("CREATE TYPE [IdList] AS TABLE ([Id] int NOT NULL PRIMARY KEY)");
                }
                catch
                {
                    // do nothing                    
                }

                var results = await cn.QueryTableAsync(
                    @"DECLARE @list [IdList];
                    INSERT INTO @list ([Id]) SELECT [Id] FROM @source;
                    SELECT [Id] FROM @list", new
                    {
                        source = new int[] { 1, 2, 3 }.ToDataTable("IdList")
                    });

                Assert.IsTrue(results.AsEnumerable().Select(row => row.Field<int>(0)).SequenceEqual(new int[] { 1, 2, 3 }));
            }
        }
    }
}
