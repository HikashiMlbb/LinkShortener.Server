using System.Data;
using DataAccess.Contexts.Abstractions;
using DataAccess.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DataAccess.Contexts;

public class DapperContext : IDapperContext
{
    private readonly string _connectionString;
    private readonly string _masterConnectionString;

    public DapperContext(IOptions<DataAccessOptions> options)
    {
        var dataAccessOptions = options.Value;
        _connectionString = dataAccessOptions;
        _masterConnectionString = dataAccessOptions.WithDatabase("master");
    }

    public IDbConnection Create()
    {
        return new SqlConnection(_connectionString);
    }

    public IDbConnection CreateMaster()
    {
        return new SqlConnection(_masterConnectionString);
    }
}