using CodeHubX.UWP.ViewModels.Settings;
using Windows.UI.Xaml;


namespace CodeHubX.UWP.Views.Settings
{
    public sealed partial class GeneralSettingsView : SettingsDetailPageBase
    {
        private GeneralSettingsViewModel ViewModel;

        public GeneralSettingsView()
        {
            InitializeComponent();

            ViewModel = new GeneralSettingsViewModel();

            DataContext = ViewModel;
        }
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != null)
                TryNavigateBackForDesktopState(e.NewState.Name);
        }
    }
}
