using CodeHubX.Helpers;
using CodeHubX.Services;
using CodeHubX.Strings;
using CodeHubX.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CodeHubX
{
	public partial class App : Application
	{
		private static MasterDetailPage mainPage;
		private static NavigationPage noNetworkPage;
		private static NavigationPage detailsPage;
		private static NavigationPage currentPage;

		private INetworkService _NetworkSvc;

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

			DependencyService.Register<INetworkService, NetworkService>();

			_NetworkSvc = DependencyService.Get<INetworkService>();

			mainPage = new MainPage();
			detailsPage = new NavigationPage(new ItemsPage());
			noNetworkPage = new NavigationPage(new NoNetworkPage());

			mainPage.Detail = _NetworkSvc.IsConnected ? detailsPage : noNetworkPage;
			currentPage = new NavigationPage(mainPage.Detail);

			MainPage = mainPage;

			//MainPage = _NetworkSvc.IsConnected
			//	    ? mainPage
			//	    : noNetworkPage;
		}

		private void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
		{
			GlobalHelper.IsConnected = e.IsConnected;
			//if (e.IsConnected)
			//{
			//	currentPage = MenuPage.CurrentPage;
			//}
			//else
			//{
			//	detailsPage = new NavigationPage(mainPage.Detail);				
			//	currentPage = noNetworkPage;
			//}
			mainPage.Detail = e.IsConnected ? MenuPage.CurrentPage : new NavigationPage(new NoNetworkPage());
		}

		private void ConnectionTypeChanged(object sender, ConnectivityTypeChangedEventArgs e)
		{

		}

		protected override void OnStart()
		{
			_NetworkSvc.ConnectivityChanged += ConnectivityChanged;
			_NetworkSvc.ConnectionTypeChanged += ConnectionTypeChanged;

			// Handle when your app starts
			AppCenter.Start("ios=0d2abada-0d0b-43c3-a500-832f8016b21d;android=b6f7aef7-c910-4c0e-a302-61b1f8095e9d;uwp=70adf665-fc63-4a9c-880f-b390818e93b5", typeof(Push), typeof(Analytics), typeof(Crashes));
		}

		protected override void OnSleep()
		{
			_NetworkSvc.ConnectivityChanged -= ConnectivityChanged;
			_NetworkSvc.ConnectionTypeChanged -= ConnectionTypeChanged;
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			_NetworkSvc.ConnectivityChanged += ConnectivityChanged;
			_NetworkSvc.ConnectionTypeChanged += ConnectionTypeChanged;
			// Handle when your app resumes
		}
	}
}
