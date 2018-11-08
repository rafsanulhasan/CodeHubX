using CodeHubX.Helpers;
using CodeHubX.Models;
using CodeHubX.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : MasterDetailPage
	{
		private IDictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();

		public MainPage()
		{
			InitializeComponent();

			MasterBehavior = MasterBehavior.Popover;

			MenuService.Add(0, (NavigationPage) Detail);

			GlobalHelper.MenuPages.ForEach((KeyValuePair<int, NavigationPage> mp)
				=> MenuService.Add(mp.Key, mp.Value));			
		}

		public async Task NavigateFromMenu(int id)
		{

			var newPageItem = MenuService.Get(id);
			var newPage = newPageItem.Page;

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