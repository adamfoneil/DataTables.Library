using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AdoUtil
{
    public static partial class AdoUtil
    {
        public static async Task<DataRow> QueryRowAsync(SqlCommand command)
        {
            var table = await QueryTableAsync(command);
            return table.Rows[0];
        }

        public static async Task<DataRow> QueryRowAsync(this SqlConnection connection, string selectQuery, Action<SqlCommand> setParameters = null)
        {
            using (var cmd = new SqlCommand(selectQuery, connection))
            {
                setParameters?.Invoke(cmd);
                return await QueryRowAsync(cmd);
            }
        }

        public static async Task<DataTable> QueryTableAsync(SqlCommand command)
        {
            DataTable result = null;
            await Task.Run(() =>
            {
                result = QueryTable(command);
            });
            return result;
        }

        public static async Task<DataTable> QueryTableAsync(this SqlConnection connection, string selectQuery, Action<SqlCommand> setParameters = null)
        {
            using (var cmd = new SqlCommand(selectQuery, connection))
            {
                setParameters?.Invoke(cmd);
                return await QueryTableAsync(cmd);
            }
        }

        public static async Task<T> QueryValueAsync<T>(this SqlConnection connection, string selectQuery, Action<SqlCommand> setParameters = null)
        {
            var table = await QueryTableAsync(connection, selectQuery, setParameters);
            return table.Rows[0].Field<T>(table.Columns[0]);
        }

        public static async Task<bool> QueryRowExistsAsync(this SqlConnection connection, string selectQuery, Action<SqlCommand> setParameters = null)
        {
            var row = await QueryRowAsync(connection, selectQuery, setParameters);
            return (row != null);
        }

        public static async Task ExecuteAsync(this SqlConnection connection, string commandText, CommandType commandType = CommandType.Text, Action<SqlCommand> setParameters = null)
        {
            if (connection.State == ConnectionState.Closed) connection.Open();
            using (var cmd = new SqlCommand(commandText, connection))
            {
                cmd.CommandType = commandType;
                setParameters?.Invoke(cmd);
                await Task.Run(() => cmd.ExecuteNonQuery());
            }
        }
    }
}
