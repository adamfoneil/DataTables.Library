[![Build status](https://ci.appveyor.com/api/projects/status/jff4nlapxigmxd1q?svg=true)](https://ci.appveyor.com/project/adamosoftware/datatables-library)
[![Nuget](https://img.shields.io/nuget/v/DataTables.Library)](https://www.nuget.org/packages/DataTables.Library/)

Most of the time I use Dapper (in some form or another, mainly through my [Dapper.CX](https://github.com/adamosoftware/Dapper.CX) or [Dapper.QX](https://github.com/adamfoneil/Dapper.QX) libraries for all data access. Sometimes, I need to use ADO.NET DataTables for really dynamic things, such as in my [SqlIntegration](https://github.com/adamosoftware/SqlIntegration) project. The syntax for querying DataTables isn't terribly convenient, so that's why this library exists.

Nuget package: **DataTables.Library** (formerly AdoUtil.Library)

Version 2.x of this library offers these extension methods:

### DataTables.Library.IEnumerableExtensions [IEnumerableExtensions.cs](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/IEnumerableExtensions.cs#L9)
- DataTable [ToDataTable](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/IEnumerableExtensions.cs#L14)
 (this IEnumerable<T> enumerable, [ bool simpleTypesOnly ])

### DataTables.Library.SqlConnectionExtensions [SqlConnectionExtensions.cs](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/SqlConnectionExtensions.cs#L7)
- DataTable [QueryTable](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/SqlConnectionExtensions.cs#L9)
 (this SqlConnection connection, string sql, [ object parameters ])
- Task\<DataTable\> [QueryTableAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/SqlConnectionExtensions.cs#L12)
 (this SqlConnection connection, string sql, [ object parameters ])

### DataTables.Library.Abstract.QueryRunner [QueryRunner.cs](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L9)
- DataTable [QueryTable](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L15)
 (IDbConnection connection, string sql, [ object parameters ])
- Task\<DataTable\> [QueryTableAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L28)
 (IDbConnection connection, string sql, [ object parameters ])

The `Query-` methods support Dapper-style anonymous object parameters, as seen [here](https://github.com/adamosoftware/AdoUtil/blob/master/Testing/QueryTableTests.cs#L29) and [here](https://github.com/adamosoftware/AdoUtil/blob/master/Testing/QueryTableTests.cs#L49). You can also pass a dictionary as query parameters, as seen [here](https://github.com/adamosoftware/AdoUtil/blob/master/Testing/QueryTableTests.cs#L59) and [here](https://github.com/adamosoftware/AdoUtil/blob/master/Testing/QueryTableTests.cs#L72).

The version 1.x library has some other methods I never really used, and the parameter handling wasn't very pretty. Version 2 boiled it down to essentials, added tests, and improved parameter handling.
