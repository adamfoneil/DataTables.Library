using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataTables.Library.Abstract
{
    public abstract class QueryRunner
    {        
        protected abstract SqlDataAdapter GetAdapter(IDbCommand command);
        protected abstract IDbCommand GetCommand(string sql, IDbConnection connection);
        protected abstract void AddParameter(IDbCommand command, string name, object value);

        public DataTable QueryTable(IDbConnection connection, string sql, object parameters = null)
        {
            using (var cmd = BuildCommand(connection, sql, parameters))
            {
                using (var adapter = GetAdapter(cmd))
                {
                    DataTable result = new DataTable();
                    adapter.Fill(result);
                    return result;
                }
            }
        }

        public async Task<DataTable> QueryTableAsync(IDbConnection connection, string sql, object parameters = null)
        {
            DataTable result = null;

            await Task.Run(() =>
            {
                result = QueryTable(connection, sql, parameters);
            });

            return result;
        }

        private IDbCommand BuildCommand(IDbConnection connection, string sql, object parameters)
        {
            IDbCommand cmd = GetCommand(sql, connection);

            if (parameters != null)
            {
                if (parameters is IDictionary dictionary)
                {
                    foreach (string key in dictionary.Keys)
                    {
                        AddParameter(cmd, key, dictionary[key]);                        
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
                        AddParameter(cmd, propertyInfo.Name, propertyInfo.GetValue(parameters));                        
                    }
                }
            }

            return cmd;
        }
    }
}
