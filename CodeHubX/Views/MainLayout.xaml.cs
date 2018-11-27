using Prism.Navigation;
using Xamarin.Forms;

namespace CodeHubX.Views
{
	public partial class MainLayout : MasterDetailPage, IMasterDetailPageOptions, INavigatedAware
	{
		public bool IsPresentedAfterNavigation
			//=> Device.Idiom != TargetIdiom.Phone;
			=> false;

		public MainLayout()
		{
			InitializeComponent();
			MasterBehavior = MasterBehavior.Popover;
		}

		public void OnNavigatedFrom(INavigationParameters parameters)
		{

		}

		public void OnNavigatedTo(INavigationParameters parameters)
		{
			if (Device.RuntimePlatform == Device.UWP)
			{
				NavigationPage.SetHasNavigationBar(this, false);
				NavigationPage.SetHasNavigationBar(Detail, true);
				NavigationPage.SetHasBackButton(Detail, true);
			}
		}
	}
}