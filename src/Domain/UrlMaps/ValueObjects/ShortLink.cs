using Domain.Common;
using Domain.Constraints;
using Domain.DomainErrors.ValueObjects;

namespace Domain.UrlMaps.ValueObjects;

public sealed class ShortLink : ValueObject
{
    private ShortLink(string shortLink)
    {
        Value = shortLink;
    }

    public string Value { get; }

    public static Result<ShortLink> Create(string shortLink)
    {
        if (shortLink.UsesUnallowedAlphabet() || shortLink.IsLengthOutOfRange())
            return ShortLinkErrors.ValidationFailure;

        return new ShortLink(shortLink);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

internal static class Extensions
{
    internal static bool UsesUnallowedAlphabet(this string shortLink)
    {
        return shortLink.Any(symbol => !ShortLinkConstraints.AllowedAlphabet.Contains(symbol));
    }

    internal static bool IsLengthOutOfRange(this string shortLink)
    {
        return shortLink.Length is < ShortLinkConstraints.MinLength or > ShortLinkConstraints.MaxLength;
    }
}