using System.Collections;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataTables.Library
{
    public static class SqlConnectionExtensions
    {
        public static DataTable QueryTable(this SqlConnection connection, string sql, object parameters = null)
        {
            using (var cmd = BuildCommand(connection, sql, parameters))
            {
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    DataTable result = new DataTable();
                    adapter.Fill(result);
                    return result;
                }
            }
        }

        public static async Task<DataTable> QueryTableAsync(this SqlConnection connection, string sql, object parameters = null)
        {
            DataTable result = null;

            await Task.Run(() =>
            { 
                result = QueryTable(connection, sql, parameters);
            });

            return result;
        }

        private static SqlCommand BuildCommand(SqlConnection connection, string sql, object parameters)
        {
            SqlCommand cmd = new SqlCommand(sql, connection);

            if (parameters != null)
            {
                if (parameters is IDictionary)
                {
                    var dictionary = parameters as IDictionary;
                    foreach (string key in dictionary.Keys)
                    {
                        cmd.Parameters.AddWithValue(key, dictionary[key]);
                    }
                }
                else
                {
                    var props = parameters.GetType()
                        .GetProperties()
                        .Where(pi => pi.CanRead && !pi.GetIndexParameters().Any())
                        .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        cmd.Parameters.AddWithValue(propertyInfo.Name, propertyInfo.GetValue(parameters));
                    }
                }
            }
            
            return cmd;
        }
    }
}
