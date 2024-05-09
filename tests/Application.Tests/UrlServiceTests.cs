using Application.Services;
using Application.Tests.Mocks;
using Application.UrlMaps;

namespace Application.Tests;

public class UrlServiceTests
{
    [Fact]
    public async Task CreatingTest()
    {
        var generator = new ShortLinkGenerator();
        var cacheRepo = new CacheRepositoryMock();
        var dbRepo = new DatabaseRepositoryMock();
        var service = new UrlService(generator, cacheRepo, dbRepo);
        const string redirectLink = "https://www.youtube.com/";

        
        var shortLinkResult = await service.CreateShortLinkAsync(redirectLink);

        if (shortLinkResult.IsFailure)
        {
            Assert.Fail(shortLinkResult.Error!.ToString());
        }
        
        var shortLink = shortLinkResult.Value!;

        var result = await service.FetchRedirectLinkAsync(shortLink);

        if (result.IsFailure)
        {
            Assert.Fail(result.Error!.ToString());
        }
        
        
        Assert.Equal(redirectLink, result.Value!);
    }
}