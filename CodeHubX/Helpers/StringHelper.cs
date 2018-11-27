namespace CodeHubX.Helpers
{
	public static class StringHelper
	{
		public static bool IsBoolString(this string @string) 
			=> @string == bool.TrueString || @string == bool.FalseString;

		public static bool IsNullOrEmptyOrWhiteSpace(this string @string) 
			=> string.IsNullOrEmpty(@string) || string.IsNullOrWhiteSpace(@string);

	}
}
