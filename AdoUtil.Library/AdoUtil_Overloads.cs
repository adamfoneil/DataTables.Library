using System;
using System.Data;
using System.Data.SqlClient;

namespace AdoUtil.Library
{    
    public static partial class AdoUtil
    {
        public static DataTable QueryTable(this SqlConnection connection, string selectQuery, object parameters)
        {
            Action<SqlCommand> action = (c) => SetParams(c, parameters);
            return QueryTable(connection, selectQuery, action);
        }

        private static void SetParams(SqlCommand command, object parameters)
        {
            var props = parameters.GetType().GetProperties();
            foreach (var p in props)
            {
                command.Parameters.AddWithValue(p.Name, p.GetValue(parameters));
            }
        }
    }
}
