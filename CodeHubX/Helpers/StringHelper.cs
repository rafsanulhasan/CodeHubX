namespace CodeHubX.Helpers
{
    public static class StringHelper
    {
        public static bool IsBoolString(this string @string)
        {
            return @string == bool.TrueString || @string == bool.FalseString;
        }

        public static bool IsNullOrEmptyOrWhiteSpace(this string @string)
        {
            return string.IsNullOrEmpty(@string) || string.IsNullOrWhiteSpace(@string);
        }
    }
}
