using System.Reflection;
using Windows.ApplicationModel.Activation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

namespace CodeHubX.UWP.Views
{
	public sealed partial class MainPage
	{
		public MainPage(IActivatedEventArgs args)
		{
			InitializeComponent();

			Forms.Init(args, new[] { Assembly.Load("CodeHubX") });
			LoadApplication(new CodeHubX.App());
		}
	}
}
