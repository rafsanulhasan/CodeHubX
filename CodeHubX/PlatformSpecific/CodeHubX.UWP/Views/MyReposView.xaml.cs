using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace CodeHubX.UWP.Views
{

	public sealed partial class MyReposView : Windows.UI.Xaml.Controls.Page
	{
		public MyReposViewmodel ViewModel { get; set; }

		private async void MyReposView_Loading(FrameworkElement sender, object args)
		{
			Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage); //Listening for Sign In message
			Messenger.Default.Register<GlobalHelper.SignOutMessageType>(this, ViewModel.RecieveSignOutMessage); //listen for sign out message
			await ViewModel.Load();
		}

		private void AllRepos_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
		{
			refreshindicator.Opacity = e.PullProgress;
			refreshindicator.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
		}

		private void StarredRepos_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
		{
			refreshindicator2.Opacity = e.PullProgress;
			refreshindicator2.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
		}

		public MyReposView()
		{
			InitializeComponent();
			ViewModel = new MyReposViewmodel();
			DataContext = ViewModel;

			Loading += MyReposView_Loading;

			NavigationCacheMode = NavigationCacheMode.Required;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			RepoListView.SelectedIndex = StarredRepoListView.SelectedIndex = -1;

			ViewModel.User = (User) e.Parameter;

			var mouseCapabilities = new MouseCapabilities();
			var hasMouse = mouseCapabilities.MousePresent != 0;

			RepoListView.IsPullToRefreshWithMouseEnabled = StarredRepoListView.IsPullToRefreshWithMouseEnabled = hasMouse;
		}

	}
}
