using CodeHubX.Services;
using CodeHubX.UWP.Services;
using System.Globalization;
using Xamarin.Forms;

[assembly: Dependency(typeof(Localizer))]
namespace CodeHubX.UWP.Services
{
	public class Localizer : ILocalizer
	{
		public CultureInfo GetCurrentCultureInfo()
			=> new CultureInfo(
				Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Languages[0].ToString());

		public void SetLocale(CultureInfo ci)
		{
		}
	}
}
