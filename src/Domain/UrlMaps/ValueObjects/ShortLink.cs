using Domain.Common;
using Domain.Constraints;

namespace Domain.UrlMaps.ValueObjects;

public sealed class ShortLink : ValueObject
{
    public string Value { get; }

    private ShortLink(string shortLink)
    {
        Value = shortLink;
    }

    public static Result<ShortLink> Create(string shortLink)
    {
        return Validate(shortLink)
            ? new ShortLink(shortLink)
            : new Error("ShortLink.ValidationFailure", "Given short link has incorrect format.");
    }

    public static bool Validate(string shortLink)
    {
        return !(shortLink.UsesUnallowedAlphabet() || shortLink.IsLengthOutOfRange());
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