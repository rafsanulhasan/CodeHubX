using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.ViewModels.Settings;
using Windows.UI.Xaml;
using static CodeHubX.Helpers.Extensions;

namespace CodeHubX.UWP.Views.Settings
{
	public sealed partial class AppearanceView : SettingsDetailPageBase
	{
		public AppearanceView()
		{
			InitializeComponent();
			DataContext = new AppearenceSettingsViewModel();
		}

		public AppearenceSettingsViewModel ViewModel => DataContext.To<AppearenceSettingsViewModel>();

		private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			if (e.NewState != null)
				TryNavigateBackForDesktopState(e.NewState.Name);
		}
	}
}
