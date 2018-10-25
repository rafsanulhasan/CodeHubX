using CodeHubX.UWP.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHubX.UWP.Views
{
	public sealed partial class CommitsView : Page
	{
		public CommitsViewmodel ViewModel;

		public CommitsView()
		{
			InitializeComponent();
			ViewModel = new CommitsViewmodel();
			DataContext = ViewModel;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e) 
			=> await ViewModel.Load(e.Parameter);
	}
}
