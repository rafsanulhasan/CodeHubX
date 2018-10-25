using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
	public sealed partial class MyOrganizationsView : Windows.UI.Xaml.Controls.Page
	{
		public MyOrganizationsViewmodel ViewModel;

		private void OrganizationsList_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
		{
			refreshindicator.Opacity = e.PullProgress;
			refreshindicator.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
		}

		public MyOrganizationsView()
		{
			InitializeComponent();

			ViewModel = new MyOrganizationsViewmodel();
			DataContext = ViewModel;

			NavigationCacheMode = NavigationCacheMode.Required;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage); //Listening for Sign In message
			Messenger.Default.Register<GlobalHelper.SignOutMessageType>(this, ViewModel.RecieveSignOutMessage); //listen for sign out message

			ViewModel.User = (User) e.Parameter;
			await ViewModel.Load();

		}
	}
}
