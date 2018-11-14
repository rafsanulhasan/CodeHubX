using CodeHubX.Helpers;
using CodeHubX.Strings;
using CodeHubX.Views;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeHubX.Services
{
	/// <summary>
	/// A navigation service that implements the IAsyncNavigationService interface
	/// </summary>
	public static class NavigationService
	{
		#region Fields & Properties
		private static MainPage _RootPage
			= Application.Current.MainPage as MainPage;

		/// <summary>
		/// Gets the internal semaphore to synchronize the navigation
		/// </summary>
		private static readonly SemaphoreSlim NavigationSemaphore
			= new SemaphoreSlim(1);

		public static Page CurrentSourcePage
			=> _RootPage.Detail;

		public static int CurrentSourcePageId
			=> MenuService.Get(CurrentSourcePage);
		#endregion

		private static async Task<Page> NavigateFromMenu(NavigationPage page, bool animated = true)
		{

			if (!animated)
				await NavigationSemaphore.WaitAsync();

			if (page != null && _RootPage.Detail != page)
			{
				_RootPage.Detail = page;

				if (Device.RuntimePlatform == Device.Android)
					await Task.Delay(100);

				_RootPage.IsPresented = false;
			}

			if (!animated)
				NavigationSemaphore.Release();

			return page;
		}

		private static void SetupPage(NavigationPage page, string title = null, ViewModelBase viewModel = null)
		{
			if (viewModel != null)
				page.BindingContext = viewModel;

			page.Title = StringHelper.IsNullOrEmptyOrWhiteSpace(title)
					 ? ChoosePageTitleByPage(page)
					 : page.Resources[title] as string;
		}

		public static Task<Page> NavigateAsync(int pageId, string title = null, ViewModelBase viewModel = null, bool animated = true)
			=> NavigateAsync(MenuService.Get(pageId), title, viewModel, animated);

		public static Task<Page> NavigateAsync(NavigationPage page, string title = null, ViewModelBase viewModel = null, bool animated = true)
		{
			SetupPage(page, title, viewModel);

			return NavigateFromMenu(page, animated);
		}

		// Checks if the app can navigate back
		public static bool CanGoBack()
			=> _RootPage.Navigation.NavigationStack.Count > 0;

		// Tries to navigate back
		public static async Task<bool> GoBackAsync()
		{
			await NavigationSemaphore.WaitAsync();

			NavigationPage currentPage;

			bool result;
			try
			{
				currentPage = (NavigationPage)await _RootPage.Navigation.PopAsync();
				currentPage = (NavigationPage)await NavigateAsync(currentPage);
				MessagingCenter.Instance.Send(currentPage, null, new GlobalHelper.SetHeaderTextMessageType { PageName = currentPage.Title });
				result = true;
			}
			catch
			{
				result = false;
			}

			NavigationSemaphore.Release();
			return result;
		}

		public static async Task<bool> GotoHomePage()
			=> (await NavigateAsync(new NavigationPage( new FeedsPage()))) != null;

		/// <summary>
		/// Search for the Page Title with the given Menu type
		/// </summary>
		/// <param name="page">type of the Menu</param>
		/// <returns>string</returns>
		/// <exception cref="Exception">When the given type don't have a Page Title pair</exception> 
		public static string ChoosePageTitleByPage(Page page)
		{
			var title = string.Empty;
			if (page is ContentPage CommentView)
				title = CommentView.Title = LangResource.pageTitle_CommentView;
			else if (page is ContentPage DeveloperProfileView)
				title = DeveloperProfileView.Title = LangResource.pageTitle_DeveloperProfileView;
			else if (page is FeedsPage feedPage)
				title = feedPage.Title = LangResource.pageTitle_FeedView;
			else if (page is ContentPage IssueDetailView)
				title = IssueDetailView.Title = LangResource.pageTitle_IssueDetailView;
			else if (page is ContentPage IssuesView)
				title = IssuesView.Title = LangResource.pageTitle_IssuesView;
			else if (page is ContentPage MyOrganizationsView)
				title = MyOrganizationsView.Title = LangResource.pageTitle_MyOrganizationsView;
			else if (page is ContentPage MyReposView)
				title = MyReposView.Title = LangResource.pageTitle_MyReposView;
			else if (page is ContentPage NotificationsView)
				title = NotificationsView.Title = LangResource.pageTitle_NotificationsView;
			else if (page is ContentPage PullRequestDetailView)
				title = PullRequestDetailView.Title = LangResource.pageTitle_PullRequestDetailView;
			else if (page is ContentPage PullRequestsView)
				title = PullRequestsView.Title = LangResource.pageTitle_PullRequestsView;
			else if (page is ContentPage RepoDetailView)
				title = RepoDetailView.Title = LangResource.pageTitle_RepoDetailView;
			else if (page is ContentPage SearchView)
				title = SearchView.Title = LangResource.pageTitle_SearchView;
			else if (page is ContentPage SettingsView)
				title = SettingsView.Title = LangResource.pageTitle_SettingsView;
			else if (page is ContentPage trendingPage)
				title = trendingPage.Title = LangResource.pageTitle_TrendingView;
			else if (page is ContentPage GeneralSettingsView)
				title = GeneralSettingsView.Title = "General";
			else if (page is ContentPage AboutSettingsView)
				title = AboutSettingsView.Title = "About";
			else if (page is ContentPage AppearanceView)
				title = AppearanceView.Title = "Appearance";
			else if (page is ContentPage DonateView)
				title = DonateView.Title = "Donate";
			else if (page is ContentPage CreditSettingsView)
				title = CreditSettingsView.Title = "Credits";
			else if (page is ContentPage CommitDetailView)
				title = CommitDetailView.Title = "Commit";
			else if (page is ContentPage CommitsView)
				title = CommitsView.Title = "Commits";

			//throw new Exception("Page Title not found for the given (Page) type: " + type);
			return title;
		}
	}
}
