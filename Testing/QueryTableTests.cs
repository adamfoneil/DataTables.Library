using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;
using AdoUtil;
using System.Collections.Generic;

namespace Testing
{
    [TestClass]
    public class QueryTableTests
    {
        private const string dbName = "AdoUtil";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            LocalDb.TryDropDatabase(dbName, out _);
        }

        [TestMethod]
        public void SyncCallNoParams()
        {
            using (var cn = LocalDb.GetConnection(dbName))
            {
                var dataTable = cn.QueryTable("SELECT * FROM [sys].[tables]");
            }
        }

        [TestMethod]
        public void SyncCallWithAnonObjParams()
        {
            using (var cn = LocalDb.GetConnection(dbName))
            {
                // won't be any data, just want to make sure no exception using parameter
                var dataTable = cn.QueryTable("SELECT * FROM [sys].[tables] WHERE [name]=@name", new { name = "fred" });
            }
        }
        
        [TestMethod]
        public void AsyncCallNoParams()
        {
            using (var cn = LocalDb.GetConnection(dbName))
            {
                var dataTable = cn.QueryTableAsync("SELECT * FROM [sys].[objects]").Result;
                Assert.IsTrue(dataTable.Rows.Count > 0);
            }
        }

        [TestMethod]
        public void AsyncCallWithAnonObjParam()
        {
            using (var cn = LocalDb.GetConnection(dbName))
            {
                var dataTable = cn.QueryTableAsync("SELECT * FROM [sys].[tables] WHERE [name]=@name", new { name = "fred" }).Result;
                Assert.IsTrue(dataTable.Columns.Count > 0);
            }
        }

        [TestMethod]
        public void SyncCallWithDictionaryParam()
        {
            using (var cn = LocalDb.GetConnection(dbName))
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
            using (var cn = LocalDb.GetConnection(dbName))
            {
                // won't be any data, just want to make sure no exception using parameter
                var dataTable = cn.QueryTableAsync("SELECT * FROM [sys].[tables] WHERE [name]=@name", new Dictionary<string, object>()
                {
                    { "name", "fred" }
                }).Result;
            }

        }
    }
}
