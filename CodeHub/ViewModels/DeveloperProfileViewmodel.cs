﻿using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace CodeHub.ViewModels
{
	public class DeveloperProfileViewmodel : AppViewmodel
	{
		#region properties

		public int ReposPaginationIndex { get; set; }
		public int StarredReposPaginationIndex { get; set; }
		public double ReposMaxScrollViewerOffset { get; set; }
		public double StarredReposMaxScrollViewerOffset { get; set; }

		private ObservableCollection<Activity> _events;
		public ObservableCollection<Activity> Events
		{
			get => _events;
			set => Set(() => Events, ref _events, value);
		}

		private ObservableCollection<Repository> _repositories;
		public ObservableCollection<Repository> Repositories
		{
			get => _repositories;
			set => Set(() => Repositories, ref _repositories, value);

		}

		private ObservableCollection<Repository> _starredRepositories;
		public ObservableCollection<Repository> StarredRepositories
		{
			get => _starredRepositories;
			set => Set(() => StarredRepositories, ref _starredRepositories, value);

		}

		private ObservableCollection<User> _followers;
		public ObservableCollection<User> Followers
		{
			get => _followers;
			set => Set(() => Followers, ref _followers, value);
		}

		private ObservableCollection<User> _following;
		public ObservableCollection<User> Following
		{
			get => _following;
			set => Set(() => Following, ref _following, value);
		}

		private bool _IsFollowersLoading;
		public bool IsFollowersLoading
		{
			get => _IsFollowersLoading;
			set => Set(() => IsFollowersLoading, ref _IsFollowersLoading, value);
		}

		private bool _IsFollowingLoading;
		public bool IsFollowingLoading
		{
			get => _IsFollowingLoading;
			set => Set(() => IsFollowingLoading, ref _IsFollowingLoading, value);
		}

		private User _developer;
		public User Developer
		{
			get => _developer;
			set => Set(() => Developer, ref _developer, value);
		}

		private bool _isFollowing;
		public bool IsFollowing
		{
			get => _isFollowing;
			set => Set(() => IsFollowing, ref _isFollowing, value);
		}

		private bool _IsEventsLoading;
		public bool IsEventsLoading
		{
			get => _IsEventsLoading;
			set => Set(() => IsEventsLoading, ref _IsEventsLoading, value);
		}

		private bool _IsReposLoading;
		public bool IsReposLoading
		{
			get => _IsReposLoading;
			set => Set(() => IsReposLoading, ref _IsReposLoading, value);
		}

		private bool _IsStarredReposLoading;
		public bool IsStarredReposLoading
		{
			get => _IsStarredReposLoading;
			set => Set(() => IsStarredReposLoading, ref _IsStarredReposLoading, value);
		}

		private bool _canFollow;
		public bool CanFollow
		{
			get => _canFollow;
			set => Set(() => CanFollow, ref _canFollow, value);
		}

		private bool _followProgress;
		public bool FollowProgress
		{
			get => _followProgress;
			set => Set(() => FollowProgress, ref _followProgress, value);
		}

		private bool _isUserEditable;
		/// <summary>
		/// Indicates whether a profile can be edited by current user
		/// </summary>
		public bool IsUserEditable
		{
			get => _isUserEditable;
			set => Set(() => IsUserEditable, ref _isUserEditable, value);
		}
		#endregion

		public async Task Load(object user)
		{
			if (GlobalHelper.IsInternet())
			{
				IsLoading = true;

				// Get the user from login name
				if (user is string login)
				{
					if (!StringHelper.IsNullOrEmptyOrWhiteSpace(login))
						Developer = await UserService.GetUserInfo(login);
				}
				else
				{
					Developer = user as User;
					if (Developer != null && Developer.Name == null)
						// Get full details of the user
						Developer = await UserService.GetUserInfo(Developer.Login);
				}


				if (Developer != null)
				{
					if (Developer.Type == AccountType.Organization || Developer.Login == GlobalHelper.UserLogin)
					{
						// Organizations can't be followed
						CanFollow = false;

						// User can edit it's own profile
						IsUserEditable = Developer.Login == GlobalHelper.UserLogin;
					}
					else
					{
						CanFollow = true;
						FollowProgress = true;
						if (await UserService.CheckFollow(Developer.Login))
							IsFollowing = true;
						FollowProgress = false;
					}

					IsEventsLoading = true;
					Events = await ActivityService.GetUserPerformedActivity(Developer.Login);
					IsEventsLoading = false;
				}
				IsLoading = false;
			}
		}

		private RelayCommand _followCommand;
		public RelayCommand FollowCommand
			=> _followCommand
			?? (_followCommand = new RelayCommand(async () =>
										  {
											  FollowProgress = true;
											  if (await UserService.FollowUser(Developer.Login))
											  {
												  IsFollowing = true;
											  }
											  FollowProgress = false;
										  }));

		private RelayCommand _unFollowCommand;
		public RelayCommand UnfollowCommand
			=> _unFollowCommand
			?? (_unFollowCommand = new RelayCommand(async () =>
										    {
											    FollowProgress = true;
											    await UserService.UnFollowUser(Developer.Login);
											    IsFollowing = false;
											    FollowProgress = false;
										    }));

		public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var p = sender as Pivot;

			switch (p.SelectedIndex)
			{
				case 0:
					IsEventsLoading = true;
					if (Developer != null)
						Events = await ActivityService.GetUserPerformedActivity(Developer.Login);

					IsEventsLoading = false;
					break;
				case 1:
					IsReposLoading = true;
					await LoadRepos();
					IsReposLoading = false;
					break;
				case 2:
					IsStarredReposLoading = true;
					await LoadStarredRepos();
					IsStarredReposLoading = false;
					break;
				case 3:
					IsFollowersLoading = true;
					Followers = await UserService.GetAllFollowers(Developer.Login);
					IsFollowersLoading = false;
					break;
				case 4:
					IsFollowingLoading = true;
					Following = await UserService.GetAllFollowing(Developer.Login);
					IsFollowingLoading = false;
					break;
			}
		}

		public async Task LoadRepos()
		{
			if (Repositories == null)
				Repositories = new ObservableCollection<Repository>();
			ReposPaginationIndex++;
			if (ReposPaginationIndex > 1)
			{
				var repos = await RepositoryUtility.GetRepositoriesForUser(Developer.Login, ReposPaginationIndex);
				if (repos != null)
				{
					if (repos.Count > 0)
						repos.ForEach(r => Repositories.Add(r));
					else
						//no more repositories to load
						ReposPaginationIndex = -1;
				}
			}
			else if (ReposPaginationIndex == 1)
				Repositories = await RepositoryUtility.GetRepositoriesForUser(Developer.Login, ReposPaginationIndex);
		}

		public async Task LoadStarredRepos()
		{
			if (StarredRepositories == null)
				StarredRepositories = new ObservableCollection<Repository>();
			StarredReposPaginationIndex++;
			if (StarredReposPaginationIndex > 1)
			{
				var repos = await RepositoryUtility.GetStarredRepositoriesForUser(Developer.Login, StarredReposPaginationIndex);
				if (repos != null)
				{
					if (repos.Count > 0)
						repos.ForEach(r => StarredRepositories.Add(r));
					else
						//no more repositories to load
						StarredReposPaginationIndex = -1;
				}
			}
			else if (StarredReposPaginationIndex == 1)
				StarredRepositories = await RepositoryUtility.GetStarredRepositoriesForUser(Developer.Login, StarredReposPaginationIndex);
		}

		public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e)
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(RepoDetailView), e.ClickedItem as Repository);

		public void UserTapped(object sender, ItemClickEventArgs e)
			=> SimpleIoc
				.Default.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(DeveloperProfileView), e.ClickedItem as User);

		public void ProfileEdit_Tapped(object sender, TappedRoutedEventArgs e)
			=> SimpleIoc
				.Default.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(EditProfileView), Developer);

		public void FeedListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var activity = e.ClickedItem as Activity;
			try
			{
				switch (activity.Type)
				{
					case "IssueCommentEvent":
						SimpleIoc
							.Default.GetInstance<IAsyncNavigationService>()
							.NavigateAsync(typeof(IssueDetailView), (activity.Repo, ((IssueCommentPayload) activity.Payload).Issue));
						break;

					case "IssuesEvent":
						SimpleIoc
							.Default.GetInstance<IAsyncNavigationService>()
							.NavigateAsync(typeof(IssueDetailView), (activity.Repo, ((IssueEventPayload) activity.Payload).Issue));
						break;

					case "PullRequestReviewCommentEvent":
						SimpleIoc
							.Default
							.GetInstance<IAsyncNavigationService>()
							.NavigateAsync(typeof(PullRequestDetailView), (activity.Repo, ((PullRequestCommentPayload) activity.Payload).PullRequest));
						break;

					case "PullRequestEvent":
					case "PullRequestReviewEvent":
						SimpleIoc
							.Default.GetInstance<IAsyncNavigationService>()
							.NavigateAsync(typeof(PullRequestDetailView), (activity.Repo, ((PullRequestEventPayload) activity.Payload).PullRequest));
						break;

					case "ForkEvent":
						SimpleIoc
							.Default
							.GetInstance<IAsyncNavigationService>()
							.NavigateAsync(typeof(RepoDetailView), ((ForkEventPayload) activity.Payload).Forkee);
						break;
					case "CommitCommentEvent":
						SimpleIoc
							.Default
							.GetInstance<IAsyncNavigationService>()
							.NavigateAsync(typeof(CommitDetailView), (activity.Repo.Id, ((CommitCommentPayload) activity.Payload).Comment.CommitId));
						break;

					case "PushEvent":
						SimpleIoc
							.Default
							.GetInstance<IAsyncNavigationService>()
							.NavigateAsync(typeof(CommitsView), (activity.Repo.Id, ((PushEventPayload) activity.Payload).Commits));
						break;

					default:
						SimpleIoc
							.Default
							.GetInstance<IAsyncNavigationService>()
							.NavigateAsync(typeof(RepoDetailView), activity.Repo);
						break;
				}
			}
			catch
			{
				SimpleIoc
					.Default
					.GetInstance<IAsyncNavigationService>()
					.NavigateAsync(typeof(RepoDetailView), activity.Repo);
			}
		}
	}
}
