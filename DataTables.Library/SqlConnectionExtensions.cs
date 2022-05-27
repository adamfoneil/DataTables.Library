#if NETSTANDARD2_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using System.Data;

using System.Threading.Tasks;

namespace DataTables.Library
{
    public static class SqlConnectionExtensions
    {
        public static DataTable QueryTable(this SqlConnection connection, string sql, object parameters = null, CommandType? commandType = null) => 
            new SqlServerQueryRunner().QueryTable(connection, sql, parameters, commandType);

        public static async Task<DataTable> QueryTableAsync(this SqlConnection connection, string sql, object parameters = null, CommandType? commandType = null) => 
            await new SqlServerQueryRunner().QueryTableAsync(connection, sql, parameters, commandType);

        public static DataTable QuerySchemaTable(this SqlConnection connection, string sql, object parameters = null) =>
            new SqlServerQueryRunner().QuerySchemaTable(connection, sql, parameters);

        public static async Task<DataTable> QuerySchemaTableAsync(this SqlConnection connection, string sql, object parameters = null) =>
            await new SqlServerQueryRunner().QuerySchemaTableAsync(connection, sql, parameters);

        public static async Task<string> SqlCreateTableAsync(this SqlConnection connection, string schema, string tableName, string sql, object parameters = null) =>
            await new SqlServerQueryRunner().SqlCreateTableAsync(connection, schema, tableName, sql, parameters);
    }
}
