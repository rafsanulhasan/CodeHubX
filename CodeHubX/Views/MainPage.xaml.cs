using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Views
{
	using Services;
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : MasterDetailPage
	{
		public MainPage()
		{
			InitializeComponent();

			MasterBehavior = MasterBehavior.Popover;

			MenuService.Add(0, (NavigationPage) Detail);
		}
	}
}