using Domain.Common;

namespace Domain.DomainErrors;

public static class ShortLinkErrors
{
    public static readonly Error ValidationFailure = new("ShortLink.ValidationFailure", "Given short link has incorrect format.");
}