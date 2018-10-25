using CodeHub.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
	public sealed partial class SearchView : Page
	{
		public SearchViewmodel ViewModel;
		private void MainSearchBox_Loaded(object sender, RoutedEventArgs e) 
			=> MainSearchBox.Focus(FocusState.Programmatic);


		public SearchView()
		{
			InitializeComponent();
			ViewModel = new SearchViewmodel();
			DataContext = ViewModel;

			NavigationCacheMode = NavigationCacheMode.Required;

			MainSearchBox.Loaded += MainSearchBox_Loaded;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			repoListView.SelectedIndex = codeListView.SelectedIndex = userListView.SelectedIndex = issueListView.SelectedIndex = -1;
		}

	}
}
