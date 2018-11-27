using CodeHubX.Models;
using CodeHubX.Services;
using CodeHubX.Views;
using System.Windows.Input;
using Xamarin.Forms;

namespace CodeHubX.ViewModels
{
	public class NavigationMenuViewmodel
	{
		public ICommand GoBackCommand { get; set; }
		public ICommand GoHomeCommand { get; set; }

		public ICommand NavigateCommand { get; set; }

		public NavigationMenuViewmodel()
		{
			GoHomeCommand = new Command(GoBack);
			GoHomeCommand = new Command(GoHome);
			NavigateCommand = new Command(Navigate);
		}

		private async void GoBack(object obj)
		{
			//await App
			//	.NavigationPage
			//	.Navigation
			//	.PopAsync();
			await NavigationService.GoBackAsync();
			App.MenuIsPresented = false;
		}

		private async void GoHome(object obj)
		{
			//App
			//	.NavigationPage
			//	.Navigation.PopToRootAsync();
			await NavigationService.GotoHomePage();
			App.MenuIsPresented = false;
		}

		private async void Navigate(object obj)
		{
			if (obj is int number)
			{
				await NavigationService.NavigateAsync(number);
			}
			else if (obj is MenuItemType type)
			{
				await NavigationService.NavigateAsync((int) type);
			}
			else
				throw new System.NotImplementedException();
			
			//App.MenuIsPresented = false;
		}
	}
}