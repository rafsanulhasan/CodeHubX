using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
	public class ContentViewmodel : AppViewmodel
	{
		private Repository _repository;
		public Repository Repository
		{
			get => _repository;
			set => Set(() => Repository, ref _repository, value);
		}

		private string _path;
		public string Path
		{
			get => _path;
			set => Set(() => Path, ref _path, value);
		}

		private string _selectedBranch;
		public string SelectedBranch
		{
			get => _selectedBranch;
			set => Set(() => SelectedBranch, ref _selectedBranch, value);
		}

		private ObservableCollection<RepositoryContentWithCommitInfo> _content;
		public ObservableCollection<RepositoryContentWithCommitInfo> Content
		{
			get => _content;
			set => Set(() => Content, ref _content, value);
		}

		public async Task Load(Tuple<Repository, string, string> repoPath)  //This page recieves Repository and Path
		{
			Repository = repoPath.Item1;
			Path = repoPath.Item2;

			if (GlobalHelper.IsInternet())
			{
				IsLoading = true;
				if (StringHelper.IsNullOrEmptyOrWhiteSpace(repoPath.Item3))
					SelectedBranch = await RepositoryUtility.GetDefaultBranch(Repository.Id);
				else
					SelectedBranch = repoPath.Item3;
				Content = await RepositoryUtility.GetRepositoryContentByPath(Repository, Path, SelectedBranch);

				IsLoading = false;

			}
		}
		public void RepoContentDrillNavigate(object sender, ItemClickEventArgs e)
		{
			RepositoryContent item = e.ClickedItem as RepositoryContentWithCommitInfo;
			if (item != null)
			{
				var navSvc = SimpleIoc
							.Default
							.GetInstance<IAsyncNavigationService>();
				if (item.Type == ContentType.File)
					navSvc
						.NavigateWithoutAnimations(typeof(FileContentView), Repository.FullName, (Repository, item.Path, SelectedBranch));
				else if (item.Type == ContentType.Dir)
					navSvc
						.NavigateWithoutAnimations(typeof(ContentView), Repository.FullName, (Repository, item.Path, SelectedBranch));
			}
		}

		private RelayCommand _repoDetailNavigateCommand;
		public RelayCommand RepoDetailNavigateCommand
			=> _repoDetailNavigateCommand
			?? (_repoDetailNavigateCommand = new RelayCommand(() =>
									   {
										   SimpleIoc
										   .Default
										   .GetInstance<IAsyncNavigationService>()
										   .NavigateAsync(typeof(RepoDetailView), Repository);
									   }));
	}
}
