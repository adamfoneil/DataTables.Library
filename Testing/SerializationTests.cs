using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;
using DataTables.Library;
using System.Linq;
using System.Data;
using Dapper;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace Testing
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void Json()
        {
            using (var cn = LocalDb.GetConnection(QueryTableTests.DbName))
            {
                const string tableName = "SerializationSample";

                try { cn.Execute($"DROP TABLE [{tableName}]"); } catch { /*do nothing*/ }

                cn.Execute($@"CREATE TABLE [{tableName}] (
                    [Column1] nvarchar(50) NOT NULL,
                    [Column2] nvarchar(50) NULL,
                    [Column3] date NULL,
                    [Column4] date NOT NULL,
                    [Column5] bit NULL,
                    [Column6] bit NOT NULL,
                    [Column7] int NULL,
                    [Column8] int NOT NULL,
                    [Column9] datetime NULL,
                    [Column10] datetime NOT NULL,
                    [Id] int identity(1,1)
                )");

                var rows = new[]
                {
                    new { Column1 = "hello", Column2 = "whatever", Column3 = new DateTime?(DateTime.Now), Column4 = DateTime.Now, Column5 = new bool?(true), Column6 = false, Column8 = 394, Column7 = new int?(294), Column9 = new DateTime?(DateTime.Now), Column10 = DateTime.Now },
                    new { Column1 = "hello", Column2 = default(string), Column3 = default(DateTime?), Column4 = DateTime.Now, Column5 = new bool?(false), Column6 = false, Column8 = 394, Column7 = default(int?), Column9 = default(DateTime?), Column10 = DateTime.Now },
                    new { Column1 = "hello", Column2 = "whatever", Column3 = new DateTime?(DateTime.Now), Column4 = DateTime.Now, Column5 = default(bool?), Column6 = false, Column8 = 394, Column7 = new int?(198), Column9 = default(DateTime?), Column10 = DateTime.Now }
                };

                foreach (var row in rows)
                {
                    cn.Execute(
                        @$"INSERT INTO [{tableName}] ([Column1], [Column2], [Column3], [Column4], [Column5], [Column6], [Column7], [Column8], [Column9], [Column10]) 
                        VALUES (@Column1, @Column2, @Column3, @Column4, @Column5, @Column6, @Column7, @Column8, @Column9, @Column10)", row);
                }

                var srcTable = cn.QueryTable($"SELECT * FROM [{tableName}]");
                var json = srcTable.SerializeAsync().Result;
                var destTable = DataTableSerializer.DeserializeAsync(json).Result;

                var srcCols = srcTable.Columns.OfType<DataColumn>().Select(col => new { Name = col.ColumnName, Type = col.DataType, IsNullable = col.AllowDBNull }).ToArray();
                var destCols = destTable.Columns.OfType<DataColumn>().Select(col => new { Name = col.ColumnName, Type = col.DataType, IsNullable = col.AllowDBNull }).ToArray();
                Assert.IsTrue(srcCols.SequenceEqual(destCols));
                Assert.IsTrue(srcTable.Rows.OfType<DataRow>().SequenceEqual(destTable.Rows.OfType<DataRow>(), new DataRowComparer()));
            }            
        }
    }

    internal class DataRowComparer : IEqualityComparer<DataRow>
    {
        public bool Equals([AllowNull] DataRow x, [AllowNull] DataRow y) => x.ItemArray.SequenceEqual(y.ItemArray);

        public int GetHashCode([DisallowNull] DataRow obj) => obj.ItemArray.GetHashCode();
    }
}
