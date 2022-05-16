using System;

namespace DataTables.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TableType : Attribute
    {
        public TableType(string typeName)
        {
            TypeName = typeName;
        }

        public string TypeName { get; }
    }
}
