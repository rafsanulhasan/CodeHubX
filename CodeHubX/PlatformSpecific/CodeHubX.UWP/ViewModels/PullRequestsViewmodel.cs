﻿using CodeHubX.Services;
using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.Services;
using CodeHubX.UWP.Views;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static CodeHubX.Helpers.CollectionHelper;

namespace CodeHubX.UWP.ViewModels
{
	public class PullRequestsViewmodel : AppViewmodel
	{
		#region properties
		private Repository _repository;
		public Repository Repository
		{
			get => _repository;
			set => Set(() => Repository, ref _repository, value);
		}

		private bool _isloadingOpen;
		public bool IsLoadingOpen  //For the first progressRing
		{
			get => _isloadingOpen;
			set => Set(() => IsLoadingOpen, ref _isloadingOpen, value);
		}

		private bool _isloadingClosed;
		public bool IsLoadingClosed   //For the second progressRing
		{
			get => _isloadingClosed;
			set => Set(() => IsLoadingClosed, ref _isloadingClosed, value);
		}

		private ObservableCollection<PullRequest> _OpenPullRequests;
		public ObservableCollection<PullRequest> OpenPullRequests
		{
			get => _OpenPullRequests;
			set => Set(() => OpenPullRequests, ref _OpenPullRequests, value);

		}

		private ObservableCollection<PullRequest> _ClosedPullRequests;
		public ObservableCollection<PullRequest> ClosedPullRequests
		{
			get => _ClosedPullRequests;
			set => Set(() => ClosedPullRequests, ref _ClosedPullRequests, value);

		}

		private bool _zeroOpenPullRequests;
		/// <summary>
		/// 'No Issues' TextBlock will display if this is true
		/// </summary>
		public bool ZeroOpenPullRequests
		{
			get => _zeroOpenPullRequests;
			set => Set(() => ZeroOpenPullRequests, ref _zeroOpenPullRequests, value);
		}

		private bool _zeroClosedPullRequests;
		/// <summary>
		/// 'No Pull Requests' TextBlock will display if this is true
		/// </summary>
		public bool ZeroClosedPullRequests
		{
			get => _zeroClosedPullRequests;
			set => Set(() => ZeroClosedPullRequests, ref _zeroClosedPullRequests, value);
		}

		private bool _isIncrementalLoadingOpen;
		public bool IsIncrementalLoadingOpen
		{
			get => _isIncrementalLoadingOpen;
			set => Set(() => IsIncrementalLoadingOpen, ref _isIncrementalLoadingOpen, value);
		}

		private bool _isIncrementalLoadingClosed;
		public bool IsIncrementalLoadingClosed
		{
			get => _isIncrementalLoadingClosed;
			set => Set(() => IsIncrementalLoadingClosed, ref _isIncrementalLoadingClosed, value);
		}

		public int OpenPaginationIndex { get; set; }

		public int ClosedPaginationIndex { get; set; }

		public double MaxOpenScrollViewerVerticalffset { get; set; }

		public double MaxClosedScrollViewerVerticalffset { get; set; }

		#endregion

		public async Task Load(Repository repository)
		{
			if (GlobalHelper.IsInternet())
			{
				Repository = repository;
				OpenPaginationIndex = ClosedPaginationIndex = 0;

				/*Clear off Pull Requests of the previous repository*/
				if (OpenPullRequests != null)
					OpenPullRequests.Clear();

				if (ClosedPullRequests != null)
					ClosedPullRequests.Clear();

				IsLoadingOpen = true;
				OpenPaginationIndex++;
				OpenPullRequests = await RepositoryUtility.GetAllPullRequestsForRepo(
									Repository.Id,
									new PullRequestRequest
									{
										State = ItemStateFilter.Open
									},
									OpenPaginationIndex
								);
				IsLoadingOpen = false;

				ZeroOpenPullRequests = OpenPullRequests.Count == 0 ? true : false;
			}
		}

		public void PullRequestTapped(object sender, ItemClickEventArgs e)
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(PullRequestDetailView), (Repository, e.ClickedItem as PullRequest));

		public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Repository != null)
			{
				var p = sender as Pivot;

				if (p.SelectedIndex == 0)
				{
					IsLoadingOpen = true;
					OpenPullRequests = await RepositoryUtility.GetAllPullRequestsForRepo(
										Repository.Id,
										new PullRequestRequest
										{
											State = ItemStateFilter.Open
										},
										OpenPaginationIndex = 1
									);
					IsLoadingOpen = false;

					ZeroOpenPullRequests = OpenPullRequests.Count == 0 ? true : false;
					MaxOpenScrollViewerVerticalffset = 0;
				}
				else if (p.SelectedIndex == 1)
				{
					IsLoadingClosed = true;

					ClosedPullRequests = await RepositoryUtility.GetAllPullRequestsForRepo(
										Repository.Id,
										new PullRequestRequest
										{
											State = ItemStateFilter.Closed
										},
										ClosedPaginationIndex = 1
									);
					IsLoadingClosed = false;

					ZeroClosedPullRequests = ClosedPullRequests.Count == 0 ? true : false;
					MaxClosedScrollViewerVerticalffset = 0;
				}
			}
		}

		public async Task OpenIncrementalLoad()
		{
			OpenPaginationIndex++;
			IsIncrementalLoadingOpen = true;
			var PRs = await RepositoryUtility.GetAllPullRequestsForRepo(
						Repository.Id,
						new PullRequestRequest
						{
							State = ItemStateFilter.Open
						},
						OpenPaginationIndex
					);

			IsIncrementalLoadingOpen = false;

			if (PRs != null)
			{
				if (PRs.Count > 0)
					PRs.ForEach(pr => OpenPullRequests.Add(pr));
				else
					//no more issues left to load
					OpenPaginationIndex = -1;
			}
		}

		public async Task ClosedIncrementalLoad()
		{
			ClosedPaginationIndex++;
			IsIncrementalLoadingClosed = true;
			var PRs = await RepositoryUtility.GetAllPullRequestsForRepo(
						Repository.Id,
						new PullRequestRequest
						{
							State = ItemStateFilter.Closed
						},
						ClosedPaginationIndex
					);

			IsIncrementalLoadingClosed = false;

			if (PRs != null)
			{
				if (PRs.Count > 0)
					PRs.ForEach(pr => ClosedPullRequests.Add(pr));
				else
					//no more issues left to load
					ClosedPaginationIndex = -1;
			}
		}
	}
}
