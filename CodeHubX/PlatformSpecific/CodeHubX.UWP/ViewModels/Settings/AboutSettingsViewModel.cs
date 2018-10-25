using CodeHubX.ViewModels.Settings;

namespace CodeHubX.UWP.ViewModels.Settings
{
	public class AboutSettingsViewModel
		: AboutSettingsViewModelBase
	{
		public AboutSettingsViewModel()
			: base()
		{
			Logo = "/Assets/Images/appLogoPurple.png";
			DisplayName = Windows.ApplicationModel.Package.Current.DisplayName;
			Publisher = Windows.ApplicationModel.Package.Current.PublisherDisplayName;

			var ver = Windows.ApplicationModel.Package.Current.Id.Version;
			Version = $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
		}
	}
}
