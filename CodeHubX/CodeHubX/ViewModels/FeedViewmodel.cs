using CodeHubX.Helpers;
using CodeHubX.Services;
using GalaSoft.MvvmLight.Command;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeHubX.ViewModels
{
	public class FeedViewmodel : AppViewmodel
	{
		private ObservableCollection<Activity> _events;
		public ObservableCollection<Activity> Events
		{
			get => _events;
			set => Set(() => Events, ref _events, value);
		}

		public int PaginationIndex { get; set; }
		public double MaxScrollViewerOffset { get; set; }

		private bool _zeroEventCount;
		public bool ZeroEventCount
		{
			get => _zeroEventCount;
			set => Set(() => ZeroEventCount, ref _zeroEventCount, value);
		}

		private bool _isIncrementalLoading;
		public bool IsIncrementalLoading
		{
			get => _isIncrementalLoading;
			set => Set(() => IsIncrementalLoading, ref _isIncrementalLoading, value);
		}

		private RelayCommand _loadCommand;
		public RelayCommand LoadCommand
			=> _loadCommand
			?? (_loadCommand = new RelayCommand(async () =>
						    {
							    if (GlobalHelper.IsInternet())
							    {
								    if (Events == null)
								    {
									    IsLoading = true;
									    await LoadEvents();
									    IsLoading = false;
								    }
							    }
						    }));
		public async void RefreshCommand(object sender, EventArgs e)
		{
			if (!GlobalHelper.IsInternet())
				//Sending NoInternet message to all viewModels
				//Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
				MessagingCenter.Instance.Send(this, null, new GlobalHelper.NoInternet().SendMessage());
			else
			{
				MessagingCenter.Instance.Send(this, null, new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
				IsLoading = true;

				PaginationIndex = 0;
				await LoadEvents();
				MaxScrollViewerOffset = 0;

				IsLoading = false;
			}
		}

		public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
		{
			IsLoggedin = false;
			User = null;
			Events = null;
		}
		public async void RecieveSignInMessage(User user)
		{
			IsLoading = true;
			if (user != null)
			{
				IsLoggedin = true;
				User = user;
				PaginationIndex = 0;
				await LoadEvents();
			}
			IsLoading = false;
		}

		public async Task LoadEvents()
		{
			PaginationIndex++;
			if (PaginationIndex > 1)
			{
				IsIncrementalLoading = true;

				var events = await UserService.GetUserActivity(PaginationIndex);
				if (events != null)
				{
					if (events.Count > 0)
						events.ForEach(e => Events.Add(e));
					else
						//no more feed items left to load
						PaginationIndex = -1;
				}
				IsIncrementalLoading = false;
			}
			else if (PaginationIndex == 1)
			{
				Events = await UserService.GetUserActivity(PaginationIndex);
				if (Events != null)
				{
					if (Events.Count == 0)
					{
						PaginationIndex = 0;
						ZeroEventCount = true;
					}
					else
						ZeroEventCount = false;
				}
			}
		}

		public void FeedListView_ItemClick(object sender, SelectedItemChangedEventArgs e)
		{
			var activity = e.SelectedItem as Activity;

			switch (activity.Type)
			{
				case "IssueCommentEvent":
					//DependencyService.Resolve<IAsyncNavigationService>()
					//.NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(activity.Repo, ((IssueCommentPayload) activity.Payload).Issue));
					break;

				case "IssuesEvent":
					//DependencyService.Resolve<IAsyncNavigationService>()
					//.NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(activity.Repo, ((IssueEventPayload) activity.Payload).Issue));
					break;

				case "PullRequestReviewCommentEvent":
					//DependencyService.Resolve<IAsyncNavigationService>()
					//.NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(activity.Repo, ((PullRequestCommentPayload) activity.Payload).PullRequest));
					break;

				case "PullRequestEvent":
				case "PullRequestReviewEvent":
					//DependencyService.Resolve<IAsyncNavigationService>()
					//.NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(activity.Repo, ((PullRequestEventPayload) activity.Payload).PullRequest));
					break;

				case "ForkEvent":
					//DependencyService.Resolve<IAsyncNavigationService>()
					//.NavigateAsync(typeof(RepoDetailView), ((ForkEventPayload) activity.Payload).Forkee);
					break;
				case "CommitCommentEvent":
					//DependencyService.Resolve<IAsyncNavigationService>()
					//.NavigateAsync(typeof(CommitDetailView), new Tuple<long, string>(activity.Repo.Id, ((CommitCommentPayload) activity.Payload).Comment.CommitId));
					break;

				case "PushEvent":
					//DependencyService.Resolve<IAsyncNavigationService>()
					//.NavigateAsync(typeof(CommitsView), new Tuple<long, IReadOnlyList<Commit>>(activity.Repo.Id, ((PushEventPayload) activity.Payload).Commits));
					break;

				default:
					//DependencyService.Resolve<IAsyncNavigationService>()
					//.NavigateAsync(typeof(RepoDetailView), activity.Repo);
					break;
			}
		}
	}
}
