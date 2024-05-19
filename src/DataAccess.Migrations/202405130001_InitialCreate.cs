using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace DataAccess.Migrations;

[Migration(202405130001)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class InitialCreate : Migration
{
    public override void Up()
    {
        Create.Table("UrlMaps")
            .WithColumn("ShortLink").AsString(10).NotNullable().PrimaryKey()
            .WithColumn("RedirectLink").AsString().NotNullable()
            .WithColumn("ExpiryDate").AsDate().Nullable()
            .WithColumn("LastVisitDate").AsDate().Nullable();
    }

    public override void Down()
    {
        Delete.Table("UrlMaps");
    }
}