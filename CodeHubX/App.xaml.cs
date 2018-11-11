using CodeHubX.Helpers;
using CodeHubX.Services;
using CodeHubX.Strings;
using CodeHubX.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
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
			AppCenter.Start("ios=c20d4e47-733f-4f27-b676-c2966d62ecff;android=2bc5d588-4a3a-4582-8f2f-70c0f164514e;uwp=b6e5e4b3-c59a-470c-92a3-ad0f10567687", typeof(Push), typeof(Analytics), typeof(Crashes));
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
