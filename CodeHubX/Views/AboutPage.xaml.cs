
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : ContentPage
	{
		public readonly string AppName = "activity_ActivityWithIssues";
		public readonly string Text = "1,2";

		public AboutPage()
			=> InitializeComponent();			
	}
}