using CodeHubX.Models;
using Plugin.Connectivity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : MasterDetailPage
	{
		public static BindableProperty IsConnectedProperty = BindableProperty.Create(nameof(IsConnected), typeof(bool), typeof(MainPage), false);

		private NavigationPage generatedNavPage;
		private Page noNetworkPage = new NoNetworkPage();

		private IDictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();

		public bool IsConnected
		{
			get => (bool) GetValue(IsConnectedProperty);
			private set => SetValue(IsConnectedProperty, value);
		}

		public MainPage()
		{
			InitializeComponent();

			MasterBehavior = MasterBehavior.Popover;

			MenuPages.Add(0, (NavigationPage) Detail);

			CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
			generatedNavPage = navPage;
		}

		private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
		{
			IsConnected = CrossConnectivity.IsSupported && e.IsConnected;

			if (IsConnected)
			{
				if (navPage.CurrentPage == noNetworkPage)
					Navigation.PopAsync();
			}
			else
			{
				if (navPage.CurrentPage != noNetworkPage)
					Navigation.PushAsync(noNetworkPage);
			}
		}

		protected override void OnDisappearing()
			=> CrossConnectivity.Current.Dispose();

		public async Task NavigateFromMenu(int id)
		{
			if (!MenuPages.ContainsKey(id))
			{
				switch (id)
				{
					case (int) MenuItemType.Browse:
						MenuPages.Add(id, new NavigationPage(new ItemsPage()));
						break;
					case (int) MenuItemType.About:
						MenuPages.Add(id, new NavigationPage(new AboutPage()));
						break;
					case (int) MenuItemType.Feeds:
						MenuPages.Add(id, new NavigationPage(new FeedsPage()));
						break;
				}
			}

			var newPage = MenuPages[id];

			if (newPage != null && Detail != newPage)
			{
				Detail = newPage;

				if (Device.RuntimePlatform == Device.Android)
					await Task.Delay(100);

				IsPresented = false;
			}
		}
	}
}