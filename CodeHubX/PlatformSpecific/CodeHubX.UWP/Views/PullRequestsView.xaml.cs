using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.ViewModels;
using Octokit;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace CodeHubX.UWP.Views
{
	public sealed partial class PullRequestsView : Windows.UI.Xaml.Controls.Page
	{
		private ScrollViewer OpenScrollViewer;
		private ScrollViewer ClosedScrollViewer;

		public PullRequestsViewmodel ViewModel { get; set; }

		private async void OnOpenScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (ViewModel.OpenPaginationIndex != -1)
			{
				var sv = (ScrollViewer) sender;

				var verticalOffset = sv.VerticalOffset;
				var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

				if ((maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset) && verticalOffset > ViewModel.MaxOpenScrollViewerVerticalffset)
				{
					ViewModel.MaxOpenScrollViewerVerticalffset = maxVerticalOffset;

					// Scrolled to bottom
					if (GlobalHelper.IsInternet())
						await ViewModel.OpenIncrementalLoad();
				}
			}


		}

		private async void OnClosedScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (ViewModel.ClosedPaginationIndex != -1)
			{
				var sv = (ScrollViewer) sender;

				var verticalOffset = sv.VerticalOffset;
				var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

				if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset && verticalOffset > ViewModel.MaxClosedScrollViewerVerticalffset)
				{
					ViewModel.MaxClosedScrollViewerVerticalffset = maxVerticalOffset;

					// Scrolled to bottom
					if (GlobalHelper.IsInternet())
						await ViewModel.ClosedIncrementalLoad();
				}
			}
		}

		private void OpenPRListView_Loaded(object sender, RoutedEventArgs e)
		{
			if (OpenScrollViewer != null)
				OpenScrollViewer.ViewChanged -= OnOpenScrollViewerViewChanged;

			OpenScrollViewer = openPRListView.FindChild<ScrollViewer>();
			OpenScrollViewer.ViewChanged += OnOpenScrollViewerViewChanged;
		}

		private void ClosedPRListView_Loaded(object sender, RoutedEventArgs e)
		{
			if (ClosedScrollViewer != null)
				ClosedScrollViewer.ViewChanged -= OnClosedScrollViewerViewChanged;

			ClosedScrollViewer = closedPRListView.FindChild<ScrollViewer>();
			ClosedScrollViewer.ViewChanged += OnClosedScrollViewerViewChanged;
		}

		private void PullRequestsView_Unloaded(object sender, RoutedEventArgs e)
		{
			if (OpenScrollViewer != null)
				OpenScrollViewer.ViewChanged -= OnOpenScrollViewerViewChanged;

			if (ClosedScrollViewer != null)
				ClosedScrollViewer.ViewChanged -= OnClosedScrollViewerViewChanged;

			OpenScrollViewer = ClosedScrollViewer = null;
		}

		public PullRequestsView()
		{
			InitializeComponent();
			ViewModel = new PullRequestsViewmodel();

			DataContext = ViewModel;

			Unloaded += PullRequestsView_Unloaded;

			NavigationCacheMode = NavigationCacheMode.Required;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.NavigationMode != NavigationMode.Back)
			{
				await ViewModel.Load((Repository) e.Parameter);
				PullRequestPivot.SelectedItem = PullRequestPivot.Items[0];
			}
		}
	}
}
