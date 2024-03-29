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

# DataTables.Library.Abstract.QueryRunner [QueryRunner.cs](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs)
## Methods
- DataSet [QueryDataSet](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L46)
 (IDbConnection connection, string sql, [ object parameters ], [ CommandType? commandType ])
- Task\<DataSet\> [QueryDataSetAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L49)
 (IDbConnection connection, string sql, [ object parameters ], [ CommandType? commandType ])
- DataTable [QueryTable](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L61)
 (IDbConnection connection, string sql, [ object parameters ], [ CommandType? commandType ])
- Task\<DataTable\> [QueryTableAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L64)
 (IDbConnection connection, string sql, [ object parameters ], [ CommandType? commandType ])
- DataTable [QuerySchemaTable](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L76)
 (IDbConnection connection, string sql, [ object parameters ])
- Task\<DataTable\> [QuerySchemaTableAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L87)
 (IDbConnection connection, string sql, [ object parameters ])
- Task\<string\> [SqlCreateTableAsync](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L99)
 (IDbConnection connection, string schema, string name, string sql, [ object parameters ])
- IEnumerable\<string\> [SqlCreateColumns](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L112)
 (DataTable schemaTable)

## Tips
- Yes, you can use table value parameters. See the [test](https://github.com/adamfoneil/DataTables.Library/blob/master/Testing/QueryTableTests.cs#L122) for an example
- The [serialization methods](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/DataTableExtensions.cs) are there because the built-in XML support does not seem to work in async methods.
- The `parameters` object on any of the methods can be in anonymous object form (Dapper-style), or you can use [Dictionary form](https://github.com/adamfoneil/DataTables.Library/blob/master/DataTables.Library/Abstract/QueryRunner.cs#L141)
- Because the `*Async` methods here use `await Task.Run`, don't use these in ASP.NET controllers, according to [.NET performance best practices](https://docs.microsoft.com/en-us/aspnet/core/performance/performance-best-practices?view=aspnetcore-3.1#avoid-blocking-calls). Use Dapper instead, which has native async methods.
