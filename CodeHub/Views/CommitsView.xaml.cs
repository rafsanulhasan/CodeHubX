using CodeHub.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
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
