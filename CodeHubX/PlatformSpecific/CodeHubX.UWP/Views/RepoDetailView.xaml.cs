using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.Services;
using CodeHubX.UWP.ViewModels;
using Octokit;
using System;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using StringHelper = CodeHubX.Helpers.StringHelper;

namespace CodeHubX.UWP.Views
{
	public sealed partial class RepoDetailView : Windows.UI.Xaml.Controls.Page
	{
		public RepoDetailViewmodel ViewModel;

		private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
		{
			/*  We are running a Java-Script function that will make all links in the WebView open in an external browser
			 *  instead of within the WebView itself.
			 */
			await ReadmeWebView.InvokeScriptAsync("eval", new[]
			{
				 @"(function()
				 {
					var body = document.getElementsByTagName('body')[0];
					var hyperlinks = document.getElementsByTagName('a');
					for(var i = 0; i < hyperlinks.length; i++)
					{
					    hyperlinks[i].setAttribute('target', '_blank');
					}
				 })()"
			});
			ReadmeLoadingRing.IsActive = false;
			ViewModel.NoReadme = false;
		}

		//private void TopScroller_OnTopScrollingRequested(object sender, EventArgs e)
		//{
		//    ReadmeScrollViewer.ChangeView(null, 0, null, false);
		//}

		private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
		{
			if (DataTransferManager.IsSupported())
			{
				if (ViewModel.Repository != null && !string.IsNullOrEmpty(ViewModel.Repository.HtmlUrl))
				{
					args.Request.Data.SetText(ViewModel.Repository.HtmlUrl);
					args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
				}
				else
				{
					args.Request.FailWithDisplayText("Nothing to share");
				}
			}

		}

		private void ShareButton_Tapped(object sender, TappedRoutedEventArgs e)
			=> DataTransferManager.ShowShareUI();

		private async void ReleasesList_ItemClick(object sender, ItemClickEventArgs e)
		{
			ReleaseBodyText.Text = (e.ClickedItem as Release).Body;
			ReleaseBodyTextPanel.Visibility = Visibility.Visible;
			await ReleaseBodyTextPanel.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
		}

		private async void CloseReleaseTextPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			await ReleaseBodyTextPanel.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
			ReleaseBodyTextPanel.Visibility = Visibility.Collapsed;
		}

		private async void Expander_Click(object sender, RoutedEventArgs e)
		{
			if (InfoPanel.Visibility == Visibility.Visible)
			{
				ExpanderIcon.Glyph = "\uE0E5";
				await InfoPanel.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 0.98f, 100, null, 0, EasingFunctionNames.SineEaseInOut);
				InfoPanel.Visibility = Visibility.Collapsed;
			}
			else
			{
				ExpanderIcon.Glyph = "\uE0E4";
				InfoPanel.SetVisualOpacity(0);
				InfoPanel.Visibility = Visibility.Visible;
				await InfoPanel.StartCompositionFadeScaleAnimationAsync(0, 1, 0.98f, 1, 100, null, 0, EasingFunctionNames.SineEaseInOut);
			}
		}

		public RepoDetailView()
		{
			//Loaded += (s, e) => TopScroller.InitializeScrollViewer(ReadmeScrollViewer);
			//Unloaded += (s, e) => TopScroller.Dispose();
			InitializeComponent();
			ViewModel = new RepoDetailViewmodel();
			DataContext = ViewModel;

			var dataTransferManager = DataTransferManager.GetForCurrentView();
			dataTransferManager.DataRequested += DataTransferManager_DataRequested;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ReleaseBodyTextPanel.Visibility = Visibility.Collapsed;

			await ViewModel.Load(e.Parameter);

			MainPivot.SelectedItem = MainPivot.Items[0];
			ReadmeLoadingRing.IsActive = true;
			ViewModel.NoReadme = true;

			if (GlobalHelper.IsInternet())
			{
				if (ViewModel.Repository != null)
				{
					var ReadmeHTML = await RepositoryUtility.GetReadmeHTMLForRepository(ViewModel.Repository.Id);
					if (!StringHelper.IsNullOrEmptyOrWhiteSpace(ReadmeHTML))
						ReadmeWebView.NavigateToString("<html><head> <link rel =\"stylesheet\" href =\"ms-appx-web:///Assets/css/github-markdown.css\" type =\"text/css\" media =\"screen\" /> </head> <body> " + ReadmeHTML + " </body></html> ");
					else
						ReadmeLoadingRing.IsActive = false;
				}
			}
			else
				ReadmeLoadingRing.IsActive = false;

		}
	}
}
