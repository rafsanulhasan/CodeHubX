using CodeHubX.UWP.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace CodeHubX.UWP.Views
{

	public sealed partial class CommitDetailView : Page
	{
		public CommitDetailViewmodel ViewModel;

		public CommitDetailView()
		{
			InitializeComponent();
			ViewModel = new CommitDetailViewmodel();

			DataContext = ViewModel;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			await ViewModel.Load(e.Parameter);
		}
	}
}
