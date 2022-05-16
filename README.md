[![Build status](https://ci.appveyor.com/api/projects/status/jff4nlapxigmxd1q?svg=true)](https://ci.appveyor.com/project/adamosoftware/datatables-library)
[![Nuget](https://img.shields.io/nuget/v/DataTables.Library)](https://www.nuget.org/packages/DataTables.Library/)

Most of the time I use Dapper in some form or another, mainly through my [Dapper.Repository](https://github.com/adamfoneil/Dapper.Repository) or [Dapper.QX](https://github.com/adamfoneil/Dapper.QX) libraries for all data access. Sometimes, I need to use ADO.NET DataTables for really dynamic things, such as in my [SqlIntegration](https://github.com/adamfoneil/SqlServerUtil) project. The syntax for querying DataTables isn't terribly convenient, so that's why this library exists.

Nuget package: **DataTables.Library** (formerly AdoUtil.Library)

Version 2.x of this library offers these extension methods:


# DataTables.Library.DataTableExtensions [DataTableExtensions.cs](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/DataTableExtensions.cs#L11)
## Methods
- Task\<string\> [SerializeAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/DataTableExtensions.cs#L16)
 (this DataTable dataTable, [ JsonSerializerOptions options ])
- Task [FromJsonAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/DataTableExtensions.cs#L45)
 (this DataTable dataTable, string json, [ JsonSerializerOptions options ])

# DataTables.Library.IEnumerableExtensions [IEnumerableExtensions.cs](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/IEnumerableExtensions.cs#L10)
## Methods
- DataTable [ToDataTable](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/IEnumerableExtensions.cs#L15)
 (this IEnumerable<T> enumerable, [ bool simpleTypesOnly ])

# DataTables.Library.SqlConnectionExtensions [SqlConnectionExtensions.cs](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/SqlConnectionExtensions.cs#L7)
## Methods
- DataTable [QueryTable](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/SqlConnectionExtensions.cs#L9)
 (this SqlConnection connection, string sql, [ object parameters ])
- Task\<DataTable\> [QueryTableAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/SqlConnectionExtensions.cs#L12)
 (this SqlConnection connection, string sql, [ object parameters ])
- DataTable [QuerySchemaTable](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/SqlConnectionExtensions.cs#L15)
 (this SqlConnection connection, string sql, [ object parameters ])
- Task\<DataTable\> [QuerySchemaTableAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/SqlConnectionExtensions.cs#L18)
 (this SqlConnection connection, string sql, [ object parameters ])
- Task\<string\> [SqlCreateTableAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/SqlConnectionExtensions.cs#L21)
 (this SqlConnection connection, string schema, string tableName, string sql, [ object parameters ])
