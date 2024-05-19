using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess.Options;

public class DataAccessOptions
{
    public const string DataAccess = "DataAccess";

    [Required] public string Server { get; set; } = null!;

    [Required] public string Database { get; set; } = null!;

    [Required] public string UserId { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;
    public TimeSpan? CacheTimeout { get; set; }

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
        var smth = new DataAccessOptions
        {
            Server = Server,
            Database = newDatabase,
            UserId = UserId,
            Password = Password,
            CacheTimeout = CacheTimeout
        };
        return smth;
    }
}