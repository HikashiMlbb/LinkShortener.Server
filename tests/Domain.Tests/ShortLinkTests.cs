using Domain.UrlMaps.ValueObjects;

namespace Domain.Tests;

public sealed class ShortLinkTests
{
    [Fact]
    public void SimpleCreating()
    {
        // Arrange
        ReadOnlySpan<string> shortLinks = ["qwerZXC", "qwerty", "QWERTzy"];
        var results = new List<bool>();

        // Act
        foreach (var shortLink in shortLinks)
        {
            results.Add(ShortLink.Create(shortLink).IsSuccess);
        }
        
        // Assert
        Assert.All(results, Assert.True);
    }

    [Fact]
    public void InvalidCreating()
    {
        // Arrange
        ReadOnlySpan<string> shortLinks = ["what", "r%ght", "qwertyqwerty"];
        var results = new List<bool>();

        // Act
        foreach (var shortLink in shortLinks)
        {
            results.Add(ShortLink.Create(shortLink).IsSuccess);
        }
        
        // Assert
        Assert.All(results, Assert.False);
    }
}