﻿using CodeHubX.Services;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CodeHubX.UWP.ViewModels
{
	public class CommitDetailViewmodel : AppViewmodel
	{
		private Repository _repository;
		public Repository Repository
		{
			get => _repository;
			set => Set(() => Repository, ref _repository, value);
		}

		private GitHubCommit _Commit;
		public GitHubCommit Commit
		{
			get => _Commit;
			set => Set(() => Commit, ref _Commit, value);
		}

		private ObservableCollection<GitHubCommitFile> _Files;
		public ObservableCollection<GitHubCommitFile> Files
		{
			get => _Files;
			set => Set(() => Files, ref _Files, value);
		}

		public async Task Load(object param)
		{
			IsLoading = true;
			if (param as Tuple<long, string> != null)
			{
				var tuple = param as Tuple<long, string>;
				Commit = await CommitService.GetCommit(tuple.Item1, tuple.Item2);
				Files = new ObservableCollection<GitHubCommitFile>(Commit.Files);
			}
			else
			{
				Commit = param as GitHubCommit;
				Files = new ObservableCollection<GitHubCommitFile>(Commit.Files);
			}
			IsLoading = false;
		}
	}
}
