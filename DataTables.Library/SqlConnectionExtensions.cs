using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace DataTables.Library
{
    public static class SqlConnectionExtensions
    {
        public static DataTable QueryTable(this SqlConnection connection, string sql, object parameters = null) => 
            new SqlServerQueryRunner().QueryTable(connection, sql, parameters);

        public static async Task<DataTable> QueryTableAsync(this SqlConnection connection, string sql, object parameters = null) => 
            await new SqlServerQueryRunner().QueryTableAsync(connection, sql, parameters);        
    }
}
