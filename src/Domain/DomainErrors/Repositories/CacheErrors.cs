using Domain.Common;

namespace Domain.DomainErrors.Repositories;

public static class CacheErrors
{
    public static readonly Error NotFound = new("Cache.NotFound", "There is no value with such key in cache.");

    public static readonly Error DeserializeFailure =
        new("Cache.DeserializeFailure", "Unable to deserialize result to such type.");
}