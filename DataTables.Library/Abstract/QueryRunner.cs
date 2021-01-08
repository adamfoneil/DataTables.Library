using DataTables.Library.Internal;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DataTables.Library.Abstract
{
    public abstract class QueryRunner
    {        
        protected abstract SqlDataAdapter GetAdapter(IDbCommand command);
        protected abstract IDbCommand GetCommand(string sql, IDbConnection connection);
        protected abstract void AddParameter(IDbCommand command, string name, object value, SqlDbType? sqlDbType = null);

        public DataTable QueryTable(IDbConnection connection, string sql, object parameters = null)
        {
            using (var cmd = BuildCommand(connection, sql, parameters))
            {
                Debug.Print($"QueryTable SQL: {sql}");
                
                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    Debug.Print($"QueryTable Param: {p.ParameterName} = {p.Value?.ToString()}");
                }

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
                        .Where(pi => pi.PropertyType.IsSimpleType() && pi.CanRead && !pi.GetIndexParameters().Any() && pi.GetValue(parameters) != null)
                        .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        var dbType = (Types.ContainsKey(propertyInfo.PropertyType)) ? Types[propertyInfo.PropertyType] : default(SqlDbType?);
                        AddParameter(cmd, propertyInfo.Name, propertyInfo.GetValue(parameters), dbType);
                    }
                }
            }

            return cmd;
        }

        private static Dictionary<Type, SqlDbType> Types
        {
            get => new Dictionary<Type, SqlDbType>()
            {
                [typeof(string)] = SqlDbType.NVarChar,
                [typeof(int)] = SqlDbType.Int,
                [typeof(int?)] = SqlDbType.Int,
                [typeof(long)] = SqlDbType.BigInt,
                [typeof(long?)] = SqlDbType.BigInt,
                [typeof(DateTime)] = SqlDbType.DateTime,
                [typeof(DateTime?)] = SqlDbType.DateTime,
                [typeof(bool)] = SqlDbType.Bit,
                [typeof(bool?)] = SqlDbType.Bit
            };
        }
    }
}
