﻿using System;

namespace DataTables.Library.Internal
{
    internal static class TypeExtensions
    {
        internal static bool IsSimpleType(this Type type) => type.Equals(typeof(string)) || type.IsValueType;

        internal static bool IsNullableGeneric(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}
