using DataAccess.Options;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DataAccess;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var databaseManager = scope.ServiceProvider.GetRequiredService<DatabaseManager>();
        var migrationManager = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        var dataAccess = scope.ServiceProvider.GetRequiredService<IOptions<DataAccessOptions>>().Value;

        databaseManager.Create(dataAccess.Database);

        migrationManager.ListMigrations();
        migrationManager.MigrateUp();

        return host;
    }
}