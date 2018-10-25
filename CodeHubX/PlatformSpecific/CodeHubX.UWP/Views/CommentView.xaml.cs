using CodeHubX.UWP.ViewModels;
using Octokit;
using Windows.UI.Xaml.Navigation;


namespace CodeHubX.UWP.Views
{

	public sealed partial class CommentView : Windows.UI.Xaml.Controls.Page
	{
		public CommentViewmodel ViewModel;

		public CommentView()
		{
			InitializeComponent();
			ViewModel = new CommentViewmodel();
			DataContext = ViewModel;

		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel.Load(e.Parameter as IssueComment);
		}
	}
}
