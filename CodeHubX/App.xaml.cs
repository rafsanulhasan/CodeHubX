using CodeHubX.Helpers;
using CodeHubX.Models;
using CodeHubX.Services;
using CodeHubX.Strings;
using CodeHubX.ViewModels;
using CodeHubX.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Plugin.Connectivity.Abstractions;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services;
using Prism.Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CodeHubX
{
	public partial class App : PrismApplication
	{
		//private static MasterDetailPage mainPage;
		//private static NavigationPage noNetworkPage;
		//private static NavigationPage detailsPage;
		//private static NavigationPage currentPage;

		private ILocalizer _Localizer;
		private INetworkService _NetworkSvc;

		//TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
		public static string AzureBackendUrl = "http://localhost:5000";
		public static bool UseMockDataStore = true;


		/* 
		 * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
		 * This imposes a limitation in which the App class must have a default constructor. 
		 * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
		 */
		public App()
			: this(null)
		{ }

		public App(IPlatformInitializer platformInitializer)
			: base(platformInitializer)
		{ }

		private async void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
		{
			GlobalHelper.IsConnected = e.IsConnected;
			var currentPath = NavigationService.GetNavigationUriPath();
			if (e.IsConnected)
			{
				if (currentPath == "MainLayout/NoNetwork")
					//_NavSvc.NavigateAsync("/ManLayout/Items");
					await NavigationService.NavigateAsync("/MainLayout/Home");
				else
					await NavigationService.GoBackAsync();
			}
			else
			{
				await NavigationService.NavigateAsync("Nav/MainLayout/NoNetwork");
			}
		}

		private void ConnectionTypeChanged(object sender, ConnectivityTypeChangedEventArgs e)
		{

		}

		private void RegisterServices(IContainerRegistry containerRegistry)
		{
			if (UseMockDataStore)
				containerRegistry.Register<MockDataStore>();
			else
				containerRegistry.Register<AzureDataStore>();

			containerRegistry.Register<IMenuService, MenuService>();
			containerRegistry.Register<ILocalization, Localization>();
			containerRegistry.Register<INetworkService, NetworkService>();
		}

		private void RegisterViews(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterForNavigation<NavigationPage>("Nav");
			containerRegistry.RegisterForNavigation<Navigation, NavigationViewModel>();
			containerRegistry.RegisterForNavigation<CarouselPage>("Carousel");
			containerRegistry.RegisterForNavigation<TabbedPage>("Tab");
			containerRegistry.RegisterForNavigation<NoNetwork>();
			containerRegistry.RegisterForNavigation<MainLayout, MainLayoutViewModel>("MainLayout");
			containerRegistry.RegisterForNavigation<Home, HomeViewModel>("Home");
			containerRegistry.RegisterForNavigation<About, AboutViewModel>("About");
			containerRegistry.RegisterForNavigation<Feeds, FeedsViewModel>("Feeds");
		}

		protected override async void OnInitialized()
		{
			InitializeComponent();

			var localization = Container.Resolve<ILocalization>();
			_Localizer = Container.Resolve<ILocalizer>();
			_NetworkSvc = Container.Resolve<INetworkService>();
			var deviceSvc = Container.Resolve<IDeviceService>();
			var fileStorage = Container.Resolve<IFileStorage>();

			_NetworkSvc.ConnectivityChanged += ConnectivityChanged;
			GlobalHelper.IsConnected = _NetworkSvc.IsConnected;

			var ci = _Localizer.GetCurrentCultureInfo();
			LangResource.Culture = ci;
			localization.SetLocale(ci);

			Resources[L10Resources.AppDisplayName] = "CodeHub";
			Resources = localization.LocalizeResource(Resources);

			var appCenterConfiguration = await ConfigurationFactory.GetAppCenterConfiguration(fileStorage, deviceSvc);
			
			// Handle when your app starts
			AppCenter.Start(appCenterConfiguration.ToString(deviceSvc), typeof(Push), typeof(Analytics), typeof(Crashes));

			var navigateToUrl = "MainLayout?selectedMenu=Home/";
			if (deviceSvc.DeviceRuntimePlatform == Xamarin.Forms.Device.Android)
				navigateToUrl = $"{navigateToUrl}/Nav/";
			navigateToUrl += _NetworkSvc.IsConnected ? "Home?selectedTab=Feeds" : "NoNetwork";

			await NavigationService.NavigateAsync(navigateToUrl);
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			RegisterServices(containerRegistry);
			RegisterViews(containerRegistry);
		}
	}
}
