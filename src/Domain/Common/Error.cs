namespace Domain.Common;

public sealed record Error(string? Code = null, string? Description = null);