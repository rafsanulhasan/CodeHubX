using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.ViewModels;
using Octokit;
using System;
using Windows.UI.Xaml.Navigation;

namespace CodeHubX.UWP.Views
{
	public sealed partial class ContentView : Windows.UI.Xaml.Controls.Page
	{
		public ContentViewmodel ViewModel;

		public ContentView()
		{
			Loaded += (s, e) =>
			{
				TopScroller.InitializeScrollViewer(ContentListView);
			};
			InitializeComponent();
			ViewModel = new ContentViewmodel();
			DataContext = ViewModel;
			Unloaded += (s, e) =>
			{
				TopScroller.Dispose();
			};
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			//This page receives repository, path and branch
			var tuple = e.Parameter as Tuple<Repository, string, string>;

			ContentListView.SelectedIndex = -1;

			if (ViewModel.Content != null)
				ViewModel.Content.Clear();
			await ViewModel.Load(tuple);
		}

		private void TopScroller_OnTopScrollingRequested(object sender, EventArgs e)
			=> ContentListView.ScrollToTheTop();
	}
}
