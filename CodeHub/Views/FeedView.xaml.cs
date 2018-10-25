using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class FeedView : Windows.UI.Xaml.Controls.Page
	{
		private ScrollViewer _feedScrollViewer;

		public FeedViewmodel ViewModel;

		private void FeedView_Loading(FrameworkElement sender, object args)
		{
			Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage); //Listening for Sign In message
			Messenger.Default.Register<GlobalHelper.SignOutMessageType>(this, ViewModel.RecieveSignOutMessage); //listen for sign out message
		}

		private void FeedListView_Loaded(object sender, RoutedEventArgs e)
		{
			if (_feedScrollViewer != null)
				_feedScrollViewer.ViewChanged -= OnScrollViewerViewChanged;

			_feedScrollViewer = FeedListView.FindChild<ScrollViewer>();
			_feedScrollViewer.ViewChanged += OnScrollViewerViewChanged;
		}

		private void FeedView_Unloaded(object sender, RoutedEventArgs e)
		{
			if (_feedScrollViewer != null)
				_feedScrollViewer.ViewChanged -= OnScrollViewerViewChanged;

			_feedScrollViewer = null;
		}

		private void Feed_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
		{
			refreshindicator.Opacity = e.PullProgress;
			refreshindicator.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
		}

		private async void MarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e) 
			=> await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));

		private async void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (ViewModel.PaginationIndex != -1)
			{
				var sv = (ScrollViewer) sender;

				var verticalOffset = sv.VerticalOffset;
				var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

				if ((maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset) && verticalOffset > ViewModel.MaxScrollViewerOffset)
				{
					ViewModel.MaxScrollViewerOffset = maxVerticalOffset;

					// Scrolled to bottom
					if (GlobalHelper.IsInternet())
						await ViewModel.LoadEvents();
				}
			}
		}

		public FeedView()
		{
			InitializeComponent();
			ViewModel = new FeedViewmodel();
			DataContext = ViewModel;
			Loading += FeedView_Loading;

			Unloaded += FeedView_Unloaded;

			NavigationCacheMode = NavigationCacheMode.Required;
		}
	}
}
