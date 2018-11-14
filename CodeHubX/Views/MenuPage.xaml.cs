
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Views
{
	using Models;
	using Services;

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
		//private MainPage RootPage => Application.Current.MainPage as MainPage;
		public static NavigationPage CurrentPage;

		private IList<MenuItem> _MenuItems;
		//private AboutPage _AboutPage=new AboutPage();
		//private Ite
		//private FeedsPage _FeedsPage=new FeedsPage();

		public MenuPage()
		{
			InitializeComponent();

			_MenuItems = _MenuItems ?? new List<MenuItem>
			{
				new MenuItem {Id = MenuItemType.Browse, MenuTitle = "Browse", PageTitle = "Browse" },
				new MenuItem {Id = MenuItemType.About, MenuTitle = "About", PageTitle = "About us" },
				new MenuItem {Id = MenuItemType.Feeds, MenuTitle = "Feeds", PageTitle = "Latest Feeds" }
			};

			ListViewMenu.ItemsSource = _MenuItems;

			ListViewMenu.SelectedItem = _MenuItems[0];
			CurrentPage = MenuService.Get((ListViewMenu.SelectedItem as MenuItem).Number);
			ListViewMenu.ItemSelected += async (sender, e) =>
			{
				if (e.SelectedItem == null)
					return;
				var selectedMenu = (MenuItem) e.SelectedItem;
				CurrentPage = MenuService.Get(selectedMenu.Number);
				await NavigationService.NavigateAsync(selectedMenu.Number);
				//var id = (int) ((MenuItem) e.SelectedItem).Id;
				//await RootPage.NavigateFromMenu(id);
			};
		}
	}
}