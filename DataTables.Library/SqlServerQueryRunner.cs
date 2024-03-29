﻿using DataTables.Library.Abstract;
#if NETSTANDARD2_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using System;
using System.Data;

namespace DataTables.Library
{
    public class SqlServerQueryRunner : QueryRunner
    {
        protected override void AddParameter(IDbCommand command, string name, object value, SqlDbType? sqlDbType = null, Action<IDbDataParameter> action = null)
        {
            var cmd = command as SqlCommand;
            IDbDataParameter param;

            if (sqlDbType.HasValue)
            {
                param = cmd.Parameters.Add(name, sqlDbType.Value);                
                param.Value = value;                
            }
            else
            {
                param = cmd.Parameters.AddWithValue(name, value);
            }

            action?.Invoke(param);
        }

        protected override SqlDataAdapter GetAdapter(IDbCommand command) => new SqlDataAdapter(command as SqlCommand);

        protected override IDbCommand GetCommand(string sql, IDbConnection connection, CommandType? commandType)
        {
            var result = new SqlCommand(sql, connection as SqlConnection);
            if (commandType.HasValue) result.CommandType = commandType.Value;
            return result;
        }
    }
}
