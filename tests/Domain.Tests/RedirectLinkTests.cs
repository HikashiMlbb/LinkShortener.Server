using Domain.UrlMaps.ValueObjects;

namespace Domain.Tests;

public sealed class RedirectLinkTests
{
    [Fact]
    public void SimpleCreating()
    {
        // Arrange
        var redirectLinks = new List<string>
        {
            "https://www.youtube.com/search?q=blablabla",
            "http://non.secured.com",
            "https://ya.ru/"
        };

        // Act
        var results = redirectLinks
            .Select(x => RedirectLink.Create(x).IsSuccess)
            .ToArray();
        
        // Assert
        Assert.All(results, Assert.True);
    }

    [Fact]
    public void InvalidCreating()
    {
        // Arrange
        var redirectLinks = new List<string>
        {
            "htttp://somecringesite com/",
            "httpss://something.wrong/",
            "https//new.site/",
            "http://another cringe site.com"
        };

        // Act
        var results = redirectLinks
            .Select(x => RedirectLink.Create(x).IsSuccess)
            .ToArray();
        
        // Assert
        Assert.All(results, Assert.True);
    }
}