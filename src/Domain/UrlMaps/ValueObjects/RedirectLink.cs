using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.UrlMaps.ValueObjects;

public sealed partial class RedirectLink : ValueObject
{
    public string Value { get; }

    private RedirectLink(string redirectLink)
    {
        Value = redirectLink;
    }

    public static Result<RedirectLink> Create(string redirectLink)
    {
        return Validate(redirectLink) 
            ? new RedirectLink(redirectLink)
            : new Error("RedirectLink.ValidationFailure", "Given redirect link has incorrect format.");
    }

    public static bool Validate(string redirectLink)
    {
        return RedirectLinkRegex().IsMatch(redirectLink);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^https?:\/\/\S+$")]
    private static partial Regex RedirectLinkRegex();
}