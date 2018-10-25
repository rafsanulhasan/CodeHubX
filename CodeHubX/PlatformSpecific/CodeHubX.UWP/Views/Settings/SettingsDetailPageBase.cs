using CodeHubX.Services;
using GalaSoft.MvvmLight.Ioc;

namespace CodeHubX.UWP.Views.Settings
{
	public partial class SettingsDetailPageBase : Windows.UI.Xaml.Controls.Page
	{
		public void TryNavigateBackForDesktopState(string stateName)
		{
			if (stateName == "Desktop")
				if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != typeof(SettingsView))
					SimpleIoc.Default.GetInstance<IAsyncNavigationService>().GoBackAsync();
		}
	}
}