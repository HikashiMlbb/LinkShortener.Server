using Domain.Common;

namespace Domain.DomainErrors;

public static class RedirectLinkErrors
{
    public static readonly Error ValidationError = new("RedirectLink.ValidationFailure", "Given redirect link has incorrect format.");
}