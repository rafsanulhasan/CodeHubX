using Xamarin.Forms.Platform.UWP;

namespace CodeHubX.UWP.Views
{
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();

			LoadApplication(new CodeHubX.App());
		}
	}
}
