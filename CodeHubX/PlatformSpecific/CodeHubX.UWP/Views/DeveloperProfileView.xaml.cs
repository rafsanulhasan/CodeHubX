﻿using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHubX.UWP.Views
{

	public sealed partial class DeveloperProfileView : Page
	{
		private ScrollViewer RepoScrollViewer;
		private ScrollViewer StarredRepoScrollViewer;

		public DeveloperProfileViewmodel ViewModel;

		private void DeveloperProfileView_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (RepoScrollViewer != null)
				RepoScrollViewer.ViewChanged -= OnRepoScrollViewerViewChanged;
			if (StarredRepoScrollViewer != null)
				StarredRepoScrollViewer.ViewChanged -= OnStarredRepoScrollViewerViewChanged;

			RepoScrollViewer = null;
			StarredRepoScrollViewer = null;
		}

		private async void OnRepoScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (ViewModel.ReposPaginationIndex != -1)
			{
				var sv = (ScrollViewer) sender;

				var verticalOffset = sv.VerticalOffset;
				var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

				if ((maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset) && verticalOffset > ViewModel.ReposMaxScrollViewerOffset)
				{
					ViewModel.ReposMaxScrollViewerOffset = maxVerticalOffset;

					// Scrolled to bottom
					if (GlobalHelper.IsInternet())
					{
						await ViewModel.LoadRepos();
					}
				}
			}
		}

		private async void OnStarredRepoScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (ViewModel.StarredReposPaginationIndex != -1)
			{
				var sv = (ScrollViewer) sender;

				var verticalOffset = sv.VerticalOffset;
				var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

				if ((maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset) && verticalOffset > ViewModel.StarredReposMaxScrollViewerOffset)
				{
					ViewModel.StarredReposMaxScrollViewerOffset = maxVerticalOffset;

					// Scrolled to bottom
					if (GlobalHelper.IsInternet())
						await ViewModel.LoadStarredRepos();
				}
			}
		}

		private void RepositoryListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (RepoScrollViewer != null)
				RepoScrollViewer.ViewChanged -= OnRepoScrollViewerViewChanged;

			RepoScrollViewer = RepositoryListView.FindChild<ScrollViewer>();
			RepoScrollViewer.ViewChanged += OnRepoScrollViewerViewChanged;
		}
		private void StarredRepositoryListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (StarredRepoScrollViewer != null)
				StarredRepoScrollViewer.ViewChanged -= OnStarredRepoScrollViewerViewChanged;

			StarredRepoScrollViewer = StarredRepositoryListView.FindChild<ScrollViewer>();
			StarredRepoScrollViewer.ViewChanged += OnStarredRepoScrollViewerViewChanged;
		}


		public DeveloperProfileView()
		{
			InitializeComponent();
			ViewModel = new DeveloperProfileViewmodel();
			DataContext = ViewModel;

			Unloaded += DeveloperProfileView_Unloaded;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			await ViewModel.Load(e.Parameter);

			if (ViewModel.Developer != null)
			{
				if (ViewModel.Developer.Type == Octokit.AccountType.Organization)
				{
					var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

					Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = languageLoader.GetString("pageTitle_OrganizationView") });
					Pivot.Items.Remove(FollowersPivotItem);
					Pivot.Items.Remove(FollowingPivotItem);
				}
			}

		}
	}
}
