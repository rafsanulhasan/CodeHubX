using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Views
{
	using Services;
	using static Helpers.GlobalHelper;

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : MasterDetailPage
	{

		public MainPage()
		{
			InitializeComponent();

			MasterBehavior = MasterBehavior.Popover;

			//_MenuPages.Add(0, (NavigationPage)Detail);
			MenuService.Add(0, (NavigationPage) Detail);
		}
	}
}