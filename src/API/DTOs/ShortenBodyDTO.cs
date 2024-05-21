namespace API.DTOs;

public sealed class ShortenBodyDto
{
    public string? Link { get; set; }
    public TimeSpan? Expiration { get; set; }
}