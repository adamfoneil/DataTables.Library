Most of the time I use Dapper (in some form or another, mainly through my [Postulate](https://github.com/adamosoftware/Postulate) library) for all data access. Sometimes, I need to use ADO.NET DataTables for really dynamic things. The syntax for querying DataTables isn't terribly convenient, so that's why this library exists.

Nuget package: **AdoUtil.Library**

This library offers some `SqlConnection` extension methods:

- [QueryRow](https://github.com/adamosoftware/AdoUtil/blob/master/AdoUtil.Library/AdoUtil.cs#L15)
- [QueryTable](https://github.com/adamosoftware/AdoUtil/blob/master/AdoUtil.Library/AdoUtil.cs#L34)
- [QueryValue](https://github.com/adamosoftware/AdoUtil/blob/master/AdoUtil.Library/AdoUtil.cs#L43)
- [QueryRowExists](https://github.com/adamosoftware/AdoUtil/blob/master/AdoUtil.Library/AdoUtil.cs#L49)
- [Execute](https://github.com/adamosoftware/AdoUtil/blob/master/AdoUtil.Library/AdoUtil.cs#L55)
