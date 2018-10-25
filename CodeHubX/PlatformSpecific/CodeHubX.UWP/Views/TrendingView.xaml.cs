using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.ViewModels;
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHubX.UWP.Views
{
	public sealed partial class TrendingView : Page
	{
		private ScrollViewer TodayScrollViewer;
		private ScrollViewer WeekScrollViewer;
		private ScrollViewer MonthScrollViewer;

		public TrendingViewmodel ViewModel;

		private void Month_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
		{
			refreshindicator3.Opacity = e.PullProgress;
			refreshindicator3.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
		}

		private void MonthListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (MonthScrollViewer != null)
				MonthScrollViewer.ViewChanged -= OnMonthScrollViewerViewChanged;

			MonthScrollViewer = monthListView.FindChild<ScrollViewer>();
			MonthScrollViewer.ViewChanged += OnMonthScrollViewerViewChanged;
		}

		private async void OnMonthScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (ViewModel.CanLoadMoreMonth)
			{
				var sv = (ScrollViewer) sender;

				var verticalOffset = sv.VerticalOffset;
				var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

				if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset)
					// Scrolled to bottom
					if (GlobalHelper.IsInternet())
						await ViewModel.MonthIncrementalLoad();
			}
		}

		private async void OnTodayScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (ViewModel.CanLoadMoreToday)
			{
				var sv = (ScrollViewer) sender;

				var verticalOffset = sv.VerticalOffset;
				var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

				if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset)
					// Scrolled to bottom
					if (GlobalHelper.IsInternet())
						await ViewModel.TodayIncrementalLoad();
			}


		}

		private async void OnWeekScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (ViewModel.CanLoadMoreWeek)
			{
				var sv = (ScrollViewer) sender;

				var verticalOffset = sv.VerticalOffset;
				var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

				if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset)
					// Scrolled to bottom
					if (GlobalHelper.IsInternet())
						await ViewModel.WeekIncrementalLoad();
			}
		}

		private void TodayListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (TodayScrollViewer != null)
				TodayScrollViewer.ViewChanged -= OnTodayScrollViewerViewChanged;

			TodayScrollViewer = todayListView.FindChild<ScrollViewer>();
			TodayScrollViewer.ViewChanged += OnTodayScrollViewerViewChanged;
		}


		private void Today_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
		{
			refreshindicator.Opacity = e.PullProgress;
			refreshindicator.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
		}

		private void TrendingView_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (TodayScrollViewer != null)
				TodayScrollViewer.ViewChanged -= OnTodayScrollViewerViewChanged;

			if (WeekScrollViewer != null)
				WeekScrollViewer.ViewChanged -= OnWeekScrollViewerViewChanged;

			if (MonthScrollViewer != null)
				MonthScrollViewer.ViewChanged -= OnMonthScrollViewerViewChanged;

			TodayScrollViewer = WeekScrollViewer = MonthScrollViewer = null;
		}

		private void Week_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
		{
			refreshindicator2.Opacity = e.PullProgress;
			refreshindicator2.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");

		}

		private void WeekListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (WeekScrollViewer != null)
				WeekScrollViewer.ViewChanged -= OnWeekScrollViewerViewChanged;

			WeekScrollViewer = weekListView.FindChild<ScrollViewer>();
			WeekScrollViewer.ViewChanged += OnWeekScrollViewerViewChanged;
		}

		public TrendingView()
		{
			InitializeComponent();
			ViewModel = new TrendingViewmodel();
			DataContext = ViewModel;

			Unloaded += TrendingView_Unloaded;

			NavigationCacheMode = NavigationCacheMode.Required;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			todayListView.SelectedIndex = weekListView.SelectedIndex = monthListView.SelectedIndex = -1;

			var mouseCapabilities = new MouseCapabilities();
			var hasMouse = mouseCapabilities.MousePresent != 0;

			todayListView.IsPullToRefreshWithMouseEnabled = weekListView.IsPullToRefreshWithMouseEnabled = monthListView.IsPullToRefreshWithMouseEnabled = hasMouse;
		}
	}
}
