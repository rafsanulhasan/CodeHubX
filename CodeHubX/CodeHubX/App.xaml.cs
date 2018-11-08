using CodeHubX.Helpers;
using CodeHubX.Services;
using CodeHubX.Strings;
using CodeHubX.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CodeHubX
{
	public partial class App : Application
	{
		//TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
		public static string AzureBackendUrl = "http://localhost:5000";
		public static bool UseMockDataStore = true;

		public App()
		{
			//InitializeComponent();
			Resources[L10Resources.AppDisplayName] = "CodeHub";

			if (UseMockDataStore)
				DependencyService.Register<MockDataStore>();
			else
				DependencyService.Register<AzureDataStore>();

			var ci = DependencyService.Get<ILocalizer>().GetCurrentCultureInfo();
			LangResource.Culture = ci;
			L10n.SetLocale(ci);
			Resources = L10n.LocalizeResource(Resources);

			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
			//AppCenter.Start("ios={Your App Secret};android={Your App Secret};uwp={Your App Secret}", typeof(Analytics), typeof(Crashes));
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
