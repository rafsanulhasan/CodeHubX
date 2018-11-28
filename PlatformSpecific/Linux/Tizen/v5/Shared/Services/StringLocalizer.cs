using System.Globalization;

namespace CodeHubX.Services
{
	public class StringLocalizer
		: ILocalizer
	{
		public CultureInfo GetCurrentCultureInfo() => throw new System.NotImplementedException();
		public void SetLocale(CultureInfo ci) => throw new System.NotImplementedException();
	}
}
