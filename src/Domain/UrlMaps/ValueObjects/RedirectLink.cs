using System.Text.RegularExpressions;
using Domain.Common;
using Domain.DomainErrors.ValueObjects;

namespace Domain.UrlMaps.ValueObjects;

public sealed partial class RedirectLink : ValueObject
{
    private RedirectLink(string redirectLink)
    {
        Value = redirectLink;
    }

    public string Value { get; }

    public static Result<RedirectLink> Create(string redirectLink)
    {
        if (!RedirectLinkRegex().IsMatch(redirectLink)) return RedirectLinkErrors.ValidationFailure;

        return new RedirectLink(redirectLink);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^https?:\/\/\S+$")]
    private static partial Regex RedirectLinkRegex();
}