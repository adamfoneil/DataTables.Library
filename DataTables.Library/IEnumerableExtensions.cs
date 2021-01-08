using DataTables.Library.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DataTables.Library
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// adapted from https://www.codeproject.com/Articles/835519/Passing-Table-Valued-Parameters-with-Dapper
        /// </summary>
        public static DataTable ToDataTable<T>(this IEnumerable<T> enumerable, bool simpleTypesOnly = true)
        {
            DataTable dataTable = new DataTable();

            if (typeof(T).IsValueType || typeof(T).FullName.Equals("System.String"))
            {
                dataTable.Columns.Add("ValueType", typeof(T));
                foreach (T obj in enumerable) dataTable.Rows.Add(obj);
            }
            else
            {
                Func<PropertyInfo, bool> filter = (pi) => true;
                if (simpleTypesOnly) filter = (pi) => pi.PropertyType.IsSimpleType();

                var properties = typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(pi => pi.CanRead && filter(pi))
                    .ToDictionary(item => item.Name);

                foreach (string name in properties.Keys)
                {
                    var propertyType = properties[name].PropertyType;
                    var columnType = (IsNullableGeneric(propertyType)) ? propertyType.GetGenericArguments()[0] : propertyType;
                    dataTable.Columns.Add(name, columnType);
                }

                foreach (T obj in enumerable)
                {
                    dataTable.Rows.Add(properties.Select(kp => kp.Value.GetValue(obj)).ToArray());
                }
            }

            return dataTable;
        }

        private static bool IsNullableGeneric(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
