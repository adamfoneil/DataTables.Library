﻿using DataTables.Library.Internal;
#if NETSTANDARD2_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataTables.Library.Abstract
{
    public abstract class QueryRunner
    {
        protected abstract SqlDataAdapter GetAdapter(IDbCommand command);
        protected abstract IDbCommand GetCommand(string sql, IDbConnection connection, CommandType? commandType);
        protected abstract void AddParameter(IDbCommand command, string name, object value, SqlDbType? sqlDbType = null, Action<IDbDataParameter> action = null);

        private T QueryInternal<T>(IDbConnection connection, Func<SqlDataAdapter, T, int> fillAction, string sql, object parameters = null, CommandType? commandType = null, [CallerMemberName] string callingMethod = null) where T : new()
        {
            using (var cmd = BuildCommand(connection, sql, parameters, commandType))
            {
                Debug.Print($"{callingMethod} SQL: {sql}");

                foreach (IDbDataParameter p in cmd.Parameters)
                {
                    Debug.Print($"{callingMethod} Param: {p.ParameterName} = {p.Value?.ToString()}");
                }

                using (var adapter = GetAdapter(cmd))
                {
                    T result = new T();
                    int rows = fillAction.Invoke(adapter, result);
                    Debug.Print($"{callingMethod} execute: {rows:n0} affected");
                    return result;
                }
            }
        }

        public DataSet QueryDataSet(IDbConnection connection, string sql, object parameters = null, CommandType? commandType = null) =>
            QueryInternal<DataSet>(connection, (adapter, ds) => adapter.Fill(ds), sql, parameters, commandType);

        public async Task<DataSet> QueryDataSetAsync(IDbConnection connection, string sql, object parameters = null, CommandType? commandType = null)
        {
            DataSet result = null;
            
            await Task.Run(() =>
            {
                result = QueryDataSet(connection, sql, parameters, commandType);
            });

            return result;
        }        

        public DataTable QueryTable(IDbConnection connection, string sql, object parameters = null, CommandType? commandType = null) =>
            QueryInternal<DataTable>(connection, (adapter, dataTable) => adapter.Fill(dataTable), sql, parameters, commandType);

        public async Task<DataTable> QueryTableAsync(IDbConnection connection, string sql, object parameters = null, CommandType? commandType = null)
        {
            DataTable result = null;

            await Task.Run(() =>
            {
                result = QueryTable(connection, sql, parameters, commandType);
            });

            return result;
        }

        public DataTable QuerySchemaTable(IDbConnection connection, string sql, object parameters = null)
        {
            using (var cmd = BuildCommand(connection, sql, parameters))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    return reader.GetSchemaTable();
                }
            }
        }

        public async Task<DataTable> QuerySchemaTableAsync(IDbConnection connection, string sql, object parameters = null)
        {
            DataTable result = null;

            await Task.Run(() =>
            {
                result = QuerySchemaTable(connection, sql, parameters);
            });

            return result;
        }

        public async Task<string> SqlCreateTableAsync(IDbConnection connection, string schema, string name, string sql, object parameters = null)
        {
            var schemaTable = await QuerySchemaTableAsync(connection, sql, parameters);
            
            StringBuilder result = new StringBuilder();
            result.Append($"CREATE TABLE [{schema}].[{name}] (\r\n\t");
            var columns = SqlCreateColumns(schemaTable);
            result.Append(string.Join(",\r\n\t", columns) + "\r\n");
            result.AppendLine(")");

            return result.ToString();
        }

        public IEnumerable<string> SqlCreateColumns(DataTable schemaTable)
        {
            foreach (DataRow row in schemaTable.Rows)
            {
                yield return ColumnSyntax(row);
            }

            string ColumnSyntax(DataRow row)
            {
                string result = $"[{row.Field<string>("ColumnName")}]";

                result += $" {row.Field<string>("DataTypeName")}";

                if (row.Field<string>("DataTypeName").StartsWith("nvar") || row.Field<string>("DataTypeName").StartsWith("var")) result += $"({row.Field<int>("ColumnSize")})";

                if (row.Field<bool>("IsIdentity")) result += " identity(1,1)";

                result += (row.Field<bool>("AllowDbNull")) ? " NULL" : " NOT NULL";

                return result;
            }
        }

        private IDbCommand BuildCommand(IDbConnection connection, string sql, object parameters, CommandType? commandType = null)
        {
            IDbCommand cmd = GetCommand(sql, connection, commandType);

            if (parameters != null)
            {
                if (parameters is IDictionary dictionary)
                {
                    foreach (string key in dictionary.Keys)
                    {
                        AddParameter(cmd, key, dictionary[key]);                        
                    }
                    return cmd;
                }
                
                var props = parameters.GetType()
                    .GetProperties()
                    .Where(pi => pi.PropertyType.IsSimpleType() && pi.CanRead && !pi.GetIndexParameters().Any() && pi.GetValue(parameters) != null)
                    .ToArray();

                foreach (var propertyInfo in props)
                {
                    var dbType = (Types.ContainsKey(propertyInfo.PropertyType)) ? Types[propertyInfo.PropertyType] : default(SqlDbType?);
                    AddParameter(cmd, propertyInfo.Name, propertyInfo.GetValue(parameters), dbType);
                }

                var tableProps = parameters.GetType()
                    .GetProperties()
                    .Where(pi => pi.PropertyType.Equals(typeof(DataTable)));

                foreach (var propertyInfo in tableProps)
                {
                    var dataTable = propertyInfo.GetValue(parameters) as DataTable;
                    AddParameter(cmd, propertyInfo.Name, dataTable, SqlDbType.Structured, (p) =>
                    {
                        (p as SqlParameter).TypeName = dataTable.TableName;
                    });
                }                
            }

            return cmd;
        }

        private static Dictionary<Type, SqlDbType> Types => new Dictionary<Type, SqlDbType>()
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
