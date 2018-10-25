﻿using CodeHub.ViewModels.Settings;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using Windows.System;
using Windows.UI.Xaml;

namespace CodeHub.Views.Settings
{
	public sealed partial class AboutSettingsView : SettingsDetailPageBase
	{
		private AboutSettingsViewModel ViewModel;

		public AboutSettingsView()
		{
			InitializeComponent();

			ViewModel = new AboutSettingsViewModel();

			DataContext = ViewModel;
		}
		private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			if (e.NewState != null)
				TryNavigateBackForDesktopState(e.NewState.Name);
		}

		private async void RateButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) 
			=> await SystemInformation.LaunchStoreForReviewAsync();

		private async void TwitterButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) 
			=> await Launcher.LaunchUriAsync(new Uri("https://twitter.com/devaalok"));

		private async void GithubButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) 
			=> await GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), "aalok05/CodeHub");

		private async void EmailButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) 
			=> await Launcher.LaunchUriAsync(new Uri("mailto:aalok_2@live.com"));
	}
}
