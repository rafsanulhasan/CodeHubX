using System.Globalization;

namespace CodeHubX.Services
{
	public class StringLocalizer : ILocalizer
	{
		public CultureInfo GetCurrentCultureInfo()
			=> new CultureInfo(
				Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Languages[0].ToString());

		public void SetLocale(CultureInfo ci)
		{
		}
	}
}
