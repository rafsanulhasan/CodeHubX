using CodeHubX.Views;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CodeHubX.ViewModels
{
	public class HomeViewModel : ViewModelBase
	{
		private IPageDialogService _DialogService;

		private ContentPage _SelectedTab;
		public ContentPage SelectedTab
		{
			get => _SelectedTab;
			set => SetProperty(ref _SelectedTab, value);
		}

		public DelegateCommand TabChangedCommand { get; private set; }

		private IList<ContentPage> _Tabs;

		public IList<ContentPage> Tabs
		{
			get => _Tabs;
			set => SetProperty(ref _Tabs, value);
		}

		public HomeViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
			  : base(navigationService, pageDialogService)
		{
			Tabs = _Tabs ?? new List<ContentPage>
			{
				new Feeds() { Title = "Feeds" },
				new ContentPage() { Title = "Issues" },
				new ContentPage() { Title = "PRs" }
			};
			SelectedTab = _SelectedTab ?? Tabs.First();
			Title = SelectedTab.Title ?? "Feeds";
		}

		public override void OnNavigatingTo(INavigationParameters parameters)
		{
			if (parameters.ContainsKey("selectedTab"))
			{
				SelectedTab = Tabs.SingleOrDefault(t => t.Title == (string)parameters["selectedTab"]);
				Title = SelectedTab.Title;
			}
		}
	}
}
