using Domain.Common;

namespace Domain.DomainErrors.ValueObjects;

public static class RedirectLinkErrors
{
    public static readonly Error ValidationFailure =
        new("RedirectLink.ValidationFailure", "Given redirect link has incorrect format.");
}