using System.Security.Cryptography;
using Application.Services.Interfaces;
using Domain.Constraints;
using Domain.UrlMaps.ValueObjects;

namespace Application.Services;

public sealed class ShortLinkGenerator : IShortLinkGenerator
{
    public ShortLink Generate()
    {
        var randomLength = Random.Shared.Next(ShortLinkConstraints.MinLength, ShortLinkConstraints.MaxLength + 1);
        var result = RandomNumberGenerator.GetString(ShortLinkConstraints.AllowedAlphabet.ToArray(), randomLength);
        return ShortLink.Create(result).Value!;
    }
}