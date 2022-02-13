namespace Packages;

public static class ParseExtensions
{
    public static bool EqualsIgnoreCase(this string a, string b)
    {
        return a.ToLower().Equals(b.ToLower());
    }
}