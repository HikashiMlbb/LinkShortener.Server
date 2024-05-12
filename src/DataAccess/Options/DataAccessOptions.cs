using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess.Options;

public class DataAccessOptions
{
    public const string DataAccess = "DataAccess";

    [Required]
    public string Server { get; set; } = null!;
    [Required]
    public string Database { get; set; } = null!;
    [Required]
    public string UserId { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;

    public override string ToString()
    {
        return new StringBuilder()
            .Append($"Server={Server};")
            .Append($"Database={Database};")
            .Append($"User Id={UserId};")
            .Append($"Password={Password};")
            .Append("TrustServerCertificate=True;")
            .ToString();
    }

    public static implicit operator string(DataAccessOptions options)
    {
        return options.ToString();
    }

    public DataAccessOptions WithDatabase(string newDatabase)
    {
        Database = newDatabase;
        return this;
    }
}