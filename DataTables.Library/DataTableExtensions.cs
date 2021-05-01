using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataTables.Library
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// provides async serialization (because DataTable.WriteXml doesn't work in .NET Core async MVC actions)
        /// </summary>
        public static async Task<string> SerializeAsync(this DataTable dataTable, JsonSerializerOptions options = null)
        {
            var model = new DataTableModel();

            var columns = dataTable.Columns.OfType<DataColumn>();

            await Task.Run(() =>
            {
                model.Columns = columns
                    .Select(col => new DataTableModel.Column()
                    {
                        Name = col.ColumnName,
                        Type = col.DataType.FullName,
                        IsNullable = col.AllowDBNull
                    });                    

                var rows = new List<Dictionary<string, object>>();
                foreach (DataRow row in dataTable.Rows) rows.Add(ReadRow(row));
                model.Rows = rows;
            });

            return JsonSerializer.Serialize(model, options);

            Dictionary<string, object> ReadRow(DataRow dataRow) => 
                columns
                    .Select(col => new { Name = col.ColumnName, Value = dataRow[col] })
                    .ToDictionary(row => row.Name, row => row.Value);
        }

        public static async Task FromJsonAsync(this DataTable dataTable, string json, JsonSerializerOptions options = null)
        {
            DataTableModel model = null;
            try
            {
                model = JsonSerializer.Deserialize<DataTableModel>(json, options);
            }
            catch (Exception exc)
            {
                throw new Exception($"Couldn't deserialize json into type {nameof(DataTableModel)}: {exc.Message}");
            }

            dataTable.Rows.Clear();
            dataTable.Columns.Clear();

            foreach (var col in model.Columns)
            {
                var dataCol = new DataColumn(col.Name, Type.GetType(col.Type));
                dataCol.AllowDBNull = col.IsNullable;
                dataTable.Columns.Add(dataCol);
            }

            var converters = new Dictionary<string, Func<object, object>>()
            {
                ["System.DateTime"] = (val) => DateTime.Parse(val.ToString()),
                ["System.Boolean"] = (val) => bool.Parse(val.ToString()),
                ["System.Int32"] = (val) => int.Parse(val.ToString()),
                ["System.Int64"] = (val) => long.Parse(val.ToString())
            };

            await Task.Run(() =>
            {
                foreach (var row in model.Rows)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var col in model.Columns)
                    {
                        if (!IsNull(row[col.Name]))
                        {                            
                            newRow[col.Name] = (converters.ContainsKey(col.Type)) ? converters[col.Type].Invoke(row[col.Name]) : row[col.Name];
                        }
                    }                                              
                    dataTable.Rows.Add(newRow);
                }
                dataTable.AcceptChanges();
            });

            bool IsNull(object value)
            {
                if (value == null) return true;

                if (value is JsonElement ele)
                {
                    if (ele.ValueKind.Equals(JsonValueKind.Object))
                    {
                        return ele.ToString().Equals("{}");
                    }
                }

                return false;
            }
        }        
    }

    public static class DataTableSerializer
    {
        public static async Task<DataTable> DeserializeAsync(string json, JsonSerializerOptions options = null)
        {
            var result = new DataTable();
            await DataTableExtensions.FromJsonAsync(result, json, options);
            return result;
        }

        public static async Task<string> SerializeAsync(DataTable dataTable, JsonSerializerOptions options = null) =>
            await DataTableExtensions.SerializeAsync(dataTable, options);
    }

    /// <summary>
    /// provides a backing model for serialization operations
    /// </summary>
    public class DataTableModel
    {
        public IEnumerable<Column> Columns { get; set; }
        public IEnumerable<Dictionary<string, object>> Rows { get; set; }

        public class Column
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool IsNullable { get; set; }
        }        
    }
}
