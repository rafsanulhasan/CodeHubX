using System.Globalization;

namespace CodeHubX.Services
{
	public interface ILocalizer
	{
		CultureInfo GetCurrentCultureInfo();
		void SetLocale(CultureInfo ci);
	}
}
