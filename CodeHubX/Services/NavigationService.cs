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
		private static readonly IDictionary<int, (NavigationPage Page, string Title, bool? WillAnimate)> NavigationStack
			= new Dictionary<int, (NavigationPage Page, string Title, bool? WillAnimate)>();

		private static MainPage _RootPage
			= Application.Current.MainPage as MainPage;

		/// <summary>
		/// Gets the internal semaphore to synchronize the navigation
		/// </summary>
		private static readonly SemaphoreSlim NavigationSemaphore
			= new SemaphoreSlim(1);

		public static NavigationPage CurrentSourcePage
		{
			get => _RootPage.Detail as NavigationPage;
			private set => Task.WhenAll(NavigateFromMenu(MenuService.Get(value).Id));
		}

		public static int CurrentSourcePageId
		{
			get => MenuService.Get(CurrentSourcePage).Id;
			private set => Task.WhenAll(NavigateFromMenu(value));
		}

		private static async Task NavigateFromMenu(int id, bool animated = false)
		{
			var newPageItem = MenuService.Get(id);
			var newPage = newPageItem.Page;

			if (newPage != null && CurrentSourcePage != newPage)
			{
				await CurrentSourcePage.Navigation.PushAsync(newPage, animated);

				if (Device.RuntimePlatform == Device.Android)
					await Task.Delay(100);

				_RootPage.IsPresented = false;
			}
		}

		private static void SetupPage(CustomContentPage page, ObservableObject viewModel = null, string title = null)
		{
			if (viewModel != null)
				page.ViewModel = viewModel;

			page.Title = StringHelper.IsNullOrEmptyOrWhiteSpace(title)
				      ? ChoosePageTitleByPage(page)
					 : page.Resources[title] as string;
		}

		private static void SetupPage<TViewModel>(CustomContentPage<TViewModel> page, TViewModel viewModel = null, string title = null)
			where TViewModel : ObservableObject, new()
		{
			if (viewModel != null)
				page.ViewModel = viewModel;

			page.Title = StringHelper.IsNullOrEmptyOrWhiteSpace(title)
				      ? ChoosePageTitleByPage(page)
					 : page.Resources[title] as string;
		}

		public static Task NavigateAsync(CustomContentPage page, ObservableObject viewModel = null, string title = null)
		{
			SetupPage(page, viewModel, title);

			return NavigateFromMenu(MenuService.Get(page).Id, true);
		}

		public static Task NavigateAsync<TViewModel>(CustomContentPage<TViewModel> page, TViewModel viewModel = null, string title = null)
			where TViewModel : ObservableObject, new()
		{
			SetupPage(page, viewModel, title);

			return NavigateFromMenu(MenuService.Get(page).Id, true);
		}

		public static async void NavigateWithoutAnimationsAsync(CustomContentPage page, ObservableObject viewModel = null, string title = null)
		{
			await NavigationSemaphore.WaitAsync();

			SetupPage(page, viewModel, title);
			await NavigateFromMenu(MenuService.Get(page).Id, true);

			NavigationSemaphore.Release();
		}

		// Navigation with parameters without animations
		public static async void NavigateWithoutAnimationsAsync<TViewModel>(CustomContentPage<TViewModel> page, ObservableObject viewModel = null, string title = null)
			where TViewModel : ObservableObject, new()
		{
			await NavigationSemaphore.WaitAsync();

			SetupPage(page, viewModel, title);
			await NavigateFromMenu(MenuService.Get(page).Id);

			NavigationSemaphore.Release();
		}

		// Checks if the app can navigate back
		public static bool CanGoBackAsync()
			=> CurrentSourcePage.Navigation.NavigationStack.Count > 0 || _RootPage.Navigation.NavigationStack.Count > 0;

		// Tries to navigate back
		public static async Task<bool> GoBackAsync()
		{
			await NavigationSemaphore.WaitAsync();

			var stackCount = CurrentSourcePage?.Navigation.NavigationStack.Count ?? _RootPage.Navigation.NavigationStack.Count;

			bool result;
			if (stackCount > 0)
			{
				var currentPage = await CurrentSourcePage?.Navigation.PopAsync();
				MessagingCenter.Instance.Send(currentPage, null, new GlobalHelper.SetHeaderTextMessageType { PageName = currentPage.Title });
				result = true;
			}
			else
				result = false;

			NavigationSemaphore.Release();
			return result;
		}

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
			else if (page is TrendingPage trendingPage)
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
