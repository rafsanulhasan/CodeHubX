using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;

namespace CodeHubX.ViewModels
{
	public class ViewModelBase
		: BindableBase,
		INavigationAware,
		IDestructible
	{
		protected static IEventAggregator EventAggregator { get; private set; }

		public INavigationService NavigationService { get; private set; }

		public IPageDialogService PageDialogService { get; private set; }

		private string _title;
		public string Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}

		public DelegateCommand<string> NavigateCommand { get; protected set; }

		//private async Task<bool> CheckResult(ref INavigationResult result, int? count)
		//{			
		//}

		public ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService = null, IEventAggregator eventAggregator = null)
		{
			if (eventAggregator != null)
				EventAggregator = eventAggregator;
			NavigateCommand = new DelegateCommand<string>(Navigate);
			NavigationService = navigationService;
			if (pageDialogService != null)
				PageDialogService = pageDialogService;
		}

		public virtual async void GoBack()
		{
			await NavigationService.GoBackToRootAsync();
			await NavigationService.NavigateAsync("/MainLayout?selectedMenu=Home/Nav/Home?selectedTab=Feeds", null, false);
		}

		protected void Navigate(string page)
					=> NavigationService.NavigateAsync(page);

		public virtual void Destroy()
		{

		}

		public virtual void OnNavigatedFrom(INavigationParameters parameters)
		{

		}

		public virtual void OnNavigatedTo(INavigationParameters parameters)
		{

		}

		public virtual void OnNavigatingTo(INavigationParameters parameters)
		{

		}
	}
}
