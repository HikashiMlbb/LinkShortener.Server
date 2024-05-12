namespace Domain.Constraints;

public static class ShortLinkConstraints
{
    public const int MinLength = 5;
    public const int MaxLength = 7;

    public static readonly IEnumerable<char> AllowedAlphabet =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz23456789$_-+".ToCharArray();
}