using System.Globalization;
using Xamarin.Forms.Platform.WPF;

namespace CodeHubX.Services
{
	public class StringLocalizer : ILocalizer
	{
		public CultureInfo GetCurrentCultureInfo()
			=> CultureInfo.CurrentCulture;

		public void SetLocale(CultureInfo ci)
		{
		}
	}
}
