﻿using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
	public class IssuesViewmodel : AppViewmodel
	{
		#region properties
		private Repository _repository;
		public Repository Repository
		{
			get => _repository;
			set => Set(() => Repository, ref _repository, value);
		}

		private string _NewIssueTitleText;
		public string NewIssueTitleText
		{
			get => _NewIssueTitleText;
			set => Set(() => NewIssueTitleText, ref _NewIssueTitleText, value);
		}

		private string _NewIssueBodyText;
		public string NewIssueBodyText
		{
			get => _NewIssueBodyText;
			set => Set(() => NewIssueBodyText, ref _NewIssueBodyText, value);
		}

		private bool _zeroOpenIssues;
		/// <summary>
		/// 'No Issues' TextBlock will display if this is true
		/// </summary>
		public bool ZeroOpenIssues
		{
			get => _zeroOpenIssues;
			set => Set(() => ZeroOpenIssues, ref _zeroOpenIssues, value);
		}

		private bool _zeroClosedIssues;
		/// <summary>
		/// 'No Issues' TextBlock will display if this is true
		/// </summary>
		public bool ZeroClosedIssues
		{
			get => _zeroClosedIssues;
			set => Set(() => ZeroClosedIssues, ref _zeroClosedIssues, value);
		}

		private bool _zeroMyIssues;
		/// <summary>
		/// 'No Issues' TextBlock will display if this is true
		/// </summary>
		public bool ZeroMyIssues
		{
			get => _zeroMyIssues;
			set => Set(() => ZeroMyIssues, ref _zeroMyIssues, value);
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

		private bool _isloadingMine;
		public bool IsLoadingMine  //For the third progressRing
		{
			get => _isloadingMine;
			set => Set(() => IsLoadingMine, ref _isloadingMine, value);
		}

		private bool _isCreatingIssue;
		public bool IsCreatingIssue
		{
			get => _isCreatingIssue;
			set => Set(() => IsCreatingIssue, ref _isCreatingIssue, value);
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

		private ObservableCollection<Issue> _openissues;
		public ObservableCollection<Issue> OpenIssues
		{
			get => _openissues;
			set => Set(() => OpenIssues, ref _openissues, value);

		}

		private ObservableCollection<Issue> _closedissues;
		public ObservableCollection<Issue> ClosedIssues
		{
			get => _closedissues;
			set => Set(() => ClosedIssues, ref _closedissues, value);

		}

		private ObservableCollection<Issue> _myissues;
		public ObservableCollection<Issue> MyIssues
		{
			get => _myissues;
			set => Set(() => MyIssues, ref _myissues, value);

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

				/*Clear off Issues of the previous repository*/
				if (OpenIssues != null)
					OpenIssues.Clear();

				if (ClosedIssues != null)
					ClosedIssues.Clear();

				if (MyIssues != null)
					MyIssues.Clear();

				IsLoadingOpen = true;
				OpenPaginationIndex++;
				OpenIssues = await RepositoryUtility.GetAllIssuesForRepo(
								Repository.Id,
								new RepositoryIssueRequest
								{
									State = ItemStateFilter.Open
								},
								OpenPaginationIndex
						   );
				IsLoadingOpen = false;

				ZeroOpenIssues = OpenIssues.Count == 0 ? true : false;
			}
		}

		public void IssueTapped(object sender, ItemClickEventArgs e)
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(IssueDetailView), (Repository, e.ClickedItem as Issue));

		public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var p = sender as Pivot;

			if (p.SelectedIndex == 0)
			{
				IsLoadingOpen = true;
				OpenIssues = await RepositoryUtility.GetAllIssuesForRepo(
								Repository.Id,
								new RepositoryIssueRequest
								{
									State = ItemStateFilter.Open
								},
								OpenPaginationIndex = 1
						   );
				IsLoadingOpen = false;

				ZeroOpenIssues = OpenIssues.Count == 0 ? true : false;
				MaxOpenScrollViewerVerticalffset = 0;
			}
			else if (p.SelectedIndex == 1)
			{
				IsLoadingClosed = true;

				ClosedIssues = await RepositoryUtility.GetAllIssuesForRepo(
								Repository.Id, new RepositoryIssueRequest
								{
									State = ItemStateFilter.Closed
								},
								ClosedPaginationIndex = 1
							);
				IsLoadingClosed = false;

				ZeroClosedIssues = ClosedIssues.Count == 0 ? true : false;
				MaxClosedScrollViewerVerticalffset = 0;
			}
			else if (p.SelectedIndex == 2)
			{
				IsLoadingMine = true;
				MyIssues = await UserService.GetAllIssuesForRepoByUser(Repository.Id);
				IsLoadingMine = false;

				ZeroMyIssues = MyIssues.Count == 0 ? true : false;
			}
		}

		public async Task OpenIncrementalLoad()
		{
			OpenPaginationIndex++;
			IsIncrementalLoadingOpen = true;
			var issues = await RepositoryUtility.GetAllIssuesForRepo(
							Repository.Id,
							new RepositoryIssueRequest
							{
								State = ItemStateFilter.Open
							},
							OpenPaginationIndex
						);

			IsIncrementalLoadingOpen = false;

			if (issues != null)
			{
				if (issues.Count > 0)
					issues.ForEach(i=>OpenIssues.Add(i));
				else
					//no more issues left to load
					OpenPaginationIndex = -1;
			}
		}

		public async Task ClosedIncrementalLoad()
		{
			ClosedPaginationIndex++;
			IsIncrementalLoadingClosed = true;
			var issues = await RepositoryUtility.GetAllIssuesForRepo(
							Repository.Id, 
							new RepositoryIssueRequest
							{
								State = ItemStateFilter.Closed
							},
							ClosedPaginationIndex
						);

			IsIncrementalLoadingClosed = false;

			if (issues != null)
			{
				if (issues.Count > 0)
					issues.ForEach(i=>ClosedIssues.Add(i));
				else
					//no more issues left to load
					ClosedPaginationIndex = -1;
			}
		}

		private RelayCommand _CreateIssue;
		public RelayCommand CreateIssue
			=> _CreateIssue
			?? (_CreateIssue = new RelayCommand(async () 
				=>
				{
					if (!string.IsNullOrWhiteSpace(NewIssueTitleText))
					{
						var newIssue = new NewIssue(NewIssueTitleText)
						{
							Body = NewIssueBodyText
						};
						IsCreatingIssue = true;
						var issue = await IssueUtility.CreateIssue(Repository.Id, newIssue);
						IsCreatingIssue = false;
						if (issue != null)
							await SimpleIoc
									.Default
									.GetInstance<IAsyncNavigationService>()
									.NavigateAsync(typeof(IssueDetailView), (Repository, issue));
					}

				}));
	}
}
