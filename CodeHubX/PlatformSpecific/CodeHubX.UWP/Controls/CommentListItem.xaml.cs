using CodeHubX.Services;
using CodeHubX.UWP.Views;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;


namespace CodeHubX.UWP.Controls
{
	public sealed partial class CommentListItem : UserControl
	{
		public CommentListItem()
			=> InitializeComponent();

		public void User_Tapped(object sender, TappedRoutedEventArgs e)
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(DeveloperProfileView), (DataContext as IssueComment).User);
	}
}
