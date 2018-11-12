using CodeHubX.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Views
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public sealed partial class TrendingPage : TabbedPage
	{
		public static BindableProperty ViewModelProperty = BindableProperty.Create(nameof(ViewModel), typeof(TrendingViewmodel), typeof(TrendingPage));
		public TrendingViewmodel ViewModel
		{
			get => GetValue(ViewModelProperty) as TrendingViewmodel;
			set => SetValue(ViewModelProperty, value);
		}

		//private void Month_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
		//{
		//	refreshindicator3.Opacity = e.PullProgress;
		//	refreshindicator3.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
		//}

		//private void MonthListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		//{
		//	if (MonthScrollViewer != null)
		//		MonthScrollViewer.ViewChanged -= OnMonthScrollViewerViewChanged;

		//	MonthScrollViewer = monthListView.FindChild<ScrollViewer>();
		//	MonthScrollViewer.ViewChanged += OnMonthScrollViewerViewChanged;
		//}

		public TrendingPage()
		{
			InitializeComponent();
			BindingContext = ViewModel = new TrendingViewmodel();
			ViewModel.TrendingReposMonthPage = MonthlyTrendingPage;
			ViewModel.TrendingReposWeekPage = WeeklyTrendingPage;
			TrendingView.PagesChanged += ViewModel.TrendingTabbedPage_SelectionChanged;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			todayListView.SelectedItem = weekListView.SelectedItem = monthListView.SelectedItem = null;

			todayListView.IsPullToRefreshEnabled = weekListView.IsPullToRefreshEnabled = monthListView.IsPullToRefreshEnabled = true;
		}
	}
}
