﻿using CodeHub.Models;
using CodeHub.Services;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
	public sealed partial class SettingsView : Page
	{
		public SettingsViewModel ViewModel;

		private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			if (e.NewState != null)
				ViewModel.CurrentState = e.NewState.Name;

			if (ViewModel.CurrentState == "Mobile")
				if (SettingsListView.SelectedIndex != -1)
					SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateWithoutAnimations(ViewModel.SubMenus[SettingsListView.SelectedIndex].DestPage, "Settings");
		}

		private void SettingsFrame_OnSizeChanged(object sender, SizeChangedEventArgs e)
			=> FrameClip.Rect = new Rect(0, 0, settingsFrame.ActualWidth, settingsFrame.ActualHeight);

		private async void SettingsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var setting = e.ClickedItem as SettingsItem;

			if (ViewModel.CurrentState == "Mobile")
			{
				await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(setting.DestPage);

				//Loading the page in settingsFrame also so that the page is visible in Desktop mode.
				await settingsFrame.Navigate(setting.DestPage);
			}
			else
			{
				if (settingsFrame.CurrentSourcePageType != setting.DestPage)
					await settingsFrame.Navigate(setting.DestPage);
			}
		}

		public SettingsView()
		{
			InitializeComponent();

			ViewModel = new SettingsViewModel();

			DataContext = ViewModel;

			NavigationCacheMode = NavigationCacheMode.Required;

		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (Window.Current.Bounds.Width < 720)
			{
				ViewModel.CurrentState = "Mobile";
				SettingsListView.SelectedIndex = -1;
			}
			else
				ViewModel.CurrentState = "Desktop";
		}

	}
}
