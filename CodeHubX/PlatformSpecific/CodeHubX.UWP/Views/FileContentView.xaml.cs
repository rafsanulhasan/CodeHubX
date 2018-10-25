using CodeHubX.UWP.ViewModels;
using Octokit;
using System;
using Windows.UI.Xaml.Navigation;


namespace CodeHubX.UWP.Views
{
	public sealed partial class FileContentView : Windows.UI.Xaml.Controls.Page
	{
		public FileContentViewmodel ViewModel;

		public FileContentView()
		{
			InitializeComponent();
			ViewModel = new FileContentViewmodel();
			DataContext = ViewModel;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			//This page receives repository, path and branch
			var tuple = e.Parameter as Tuple<Repository, string, string>;

			await ViewModel.Load(tuple);
		}
	}

}
