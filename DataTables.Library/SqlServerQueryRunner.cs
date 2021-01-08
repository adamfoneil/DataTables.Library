using DataTables.Library.Abstract;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataTables.Library
{
    public class SqlServerQueryRunner : QueryRunner
    {
        protected override void AddParameter(IDbCommand command, string name, object value, SqlDbType? sqlDbType = null)
        {
            var cmd = command as SqlCommand;
            if (sqlDbType.HasValue)
            {
                var param = cmd.Parameters.Add(name, sqlDbType.Value);
                param.Value = value;
            }
            else
            {
                cmd.Parameters.AddWithValue(name, value);
            }            
        }

        protected override SqlDataAdapter GetAdapter(IDbCommand command) => new SqlDataAdapter(command as SqlCommand);

        protected override IDbCommand GetCommand(string sql, IDbConnection connection) => new SqlCommand(sql, connection as SqlConnection);        
    }
}
