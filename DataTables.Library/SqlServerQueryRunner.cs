using DataTables.Library.Abstract;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataTables.Library
{
    public class SqlServerQueryRunner : QueryRunner
    {
        protected override void AddParameter(IDbCommand command, string name, object value)
        {            
            (command as SqlCommand).Parameters.AddWithValue(name, value);
        }

        protected override SqlDataAdapter GetAdapter(IDbCommand command) => new SqlDataAdapter(command as SqlCommand);

        protected override IDbCommand GetCommand(string sql, IDbConnection connection) => new SqlCommand(sql, connection as SqlConnection);        
    }
}
