using DataTables.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;

namespace Testing
{
    [TestClass]
    public class ToDataTableTests
    {
        [TestMethod]
        public void StringEnumerableToDataTable()
        {
            string[] items = new string[]
            {
                "this",
                "that",
                "other"
            };

            var table = items.ToDataTable();

            Assert.IsTrue(table.Columns[0].ColumnName.Equals("ValueType"));
            Assert.IsTrue(table.Rows.Count == 3);
            Assert.IsTrue(table.AsEnumerable().Select(row => row.Field<string>("ValueType")).SequenceEqual(items));
        }

        [TestMethod]
        public void GenericEnumerableToDataTable()
        {
            var items = new[]
            {
                new { Message = "hello", Date = DateTime.Today, Flag = true },
                new { Message = "good-bye", Date = DateTime.MinValue, Flag = false }
            };

            var table = items.ToDataTable();

            Assert.IsTrue(table.Columns
                .OfType<DataColumn>().Select(col => col.ColumnName)
                .SequenceEqual(new string[] { "Message", "Date", "Flag" }));

            Assert.IsTrue(table.Rows.Count == 2);

            Assert.IsTrue(table.Rows[0]["Message"].Equals("hello"));
        }

        [TestMethod]
        public void EnumerableWithNullableToDataTable()
        {
            var items = new WithNullable[]
            {
                new WithNullable() { Message = "hello", Date = DateTime.Today, Flag = true },
                new WithNullable() { Message = "good-bye", Date = null, Flag = false }
            };

            var table = items.ToDataTable();
        }

        [TestMethod]
        public void EnumerableWithComplexNullable()
        {
            var items = new WithNullableComplex[]
            {
                new WithNullableComplex() { Message = "hello", Date = DateTime.Today, Flag = true },
                new WithNullableComplex() { Message = "good-bye", Date = null, Flag = false }
            };

            var table = items.ToDataTable();
            Assert.IsTrue(!table.Columns.Contains("WithNullable"));
            Assert.IsTrue(table.Columns.Count == 3);

            var table2 = items.ToDataTable(simpleTypesOnly: false);
            Assert.IsTrue(table2.Columns.Contains("WithNullable"));
            Assert.IsTrue(table2.Columns.Count == 4);
        }

        private class WithNullable
        {
            public string Message { get; set; }
            public DateTime? Date { get; set; }
            public bool Flag { get; set; }
        }

        private class WithNullableComplex
        {
            public string Message { get; set; }
            public DateTime? Date { get; set; }
            public bool Flag { get; set; }
            public WithNullable WithNullable { get; set; }
        }
    }
}
