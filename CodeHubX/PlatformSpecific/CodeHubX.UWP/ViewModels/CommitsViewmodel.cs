using CodeHubX.Services;
using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.Views;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHubX.UWP.ViewModels
{
	public class CommitsViewmodel : AppViewmodel
	{
		private ObservableCollection<GitHubCommit> _Commits;
		public ObservableCollection<GitHubCommit> Commits
		{
			get => _Commits;
			set => Set(() => Commits, ref _Commits, value);

		}

		public async Task Load(object param)
		{
			if (GlobalHelper.IsInternet())
			{
				IsLoading = true;
				Commits = new ObservableCollection<GitHubCommit>();
				var tuple = param as Tuple<long, IReadOnlyList<Commit>>;

				if (tuple != null)
				{
					foreach (var commit in tuple.Item2)
						Commits.Add(await CommitService.GetCommit(tuple.Item1, commit.Sha));
				}
				IsLoading = false;
			}
		}

		public void CommitList_ItemClick(object sender, ItemClickEventArgs e)
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(CommitDetailView), e.ClickedItem as GitHubCommit);
	}
}
