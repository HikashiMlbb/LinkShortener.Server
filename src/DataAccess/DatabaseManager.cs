using Dapper;
using DataAccess.Contexts.Abstractions;

namespace DataAccess;

public sealed class DatabaseManager(IDapperContext context)
{
    public void Create(string name)
    {
        const string getQuery = "SELECT name FROM sys.databases WHERE name=@name";

        using var db = context.CreateMaster();
        db.Open();

        var result = db.QueryFirstOrDefault(getQuery, new { name });

        if (result is null) db.Execute($"CREATE DATABASE {name}");
    }
}