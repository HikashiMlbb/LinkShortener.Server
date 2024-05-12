using System.Data;

namespace DataAccess.Contexts.Abstractions;

public interface IDapperContext
{
    public IDbConnection Create();
    public IDbConnection CreateMaster();
}