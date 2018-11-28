using CodeHubX.Models;
using CodeHubX.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CodeHubX.ViewModels
{
	public class MainLayoutViewModel
		: ViewModelBase
	{
		private readonly IDeviceService _DeviceService;
		private string _CurrentUrl;
		private readonly INavigationService _NavigationSvc;
		//private IFileStorage _FileStorage;
		private readonly IPageDialogService _PageDialogSvc;
		private IMenuService _MenuSvc;

		public bool IsAndroid => _DeviceService.DeviceRuntimePlatform == Device.Android;

		public bool IsUWP => _DeviceService.DeviceRuntimePlatform == Device.UWP;

		private IReadOnlyList<NavMenuItem> _Items;
		public IReadOnlyList<NavMenuItem> Items
		{
			get => _Items;
			set => SetProperty(ref _Items, value);
		}

		public new DelegateCommand<object> NavigateCommand { get; private set; }

		private NavMenuItem _SelectedItem;
		public NavMenuItem SelectedItem
		{
			get => _SelectedItem;
			set => SetProperty(ref _SelectedItem, value);
		}

		//private AppConfiguration _Configuration;
		//public AppConfiguration Configuration
		//{
		//	get => _Configuration;
		//	set => SetProperty(ref _Configuration, value);
		//}

		public MainLayoutViewModel(INavigationService navigationService, IMenuService menuService, IPageDialogService pageDialogService, IDeviceService deviceService)
		    //(INavigationService navigationService, Services.INavigationService navigationService2, IFileStorage fileStorage, IDeviceService deviceService, IPageDialogService pageDialogService)
		    : base(navigationService, pageDialogService)
		{
			//_NavSvc = navSvc;
			_MenuSvc = menuService;
			_NavigationSvc = navigationService;
			_DeviceService = deviceService;
			NavigateCommand = new DelegateCommand<object>(Navigate);

			//_FileStorage = fileStorage;
			_PageDialogSvc = pageDialogService;
			Items = _MenuSvc.Menus;
			SelectedItem = Items.First();
			Title = SelectedItem.MenuTitle;
		}

		//public async System.Threading.Tasks.Task BuildConfig(IFileStorage fileStorage)
		//	=> _Configuration = await ConfigurationFactory.Build(fileStorage);

		public async void Navigate(object e)
		{
			if (e is NavMenuItem menu)
			{
				var uri = $"MainLayout?selectedMenu={menu.MenuTitle}";
				if (_DeviceService.DeviceRuntimePlatform == Device.UWP)
				{
					uri = $"Nav/{uri}/";
				}
				else if (_DeviceService.DeviceRuntimePlatform == Device.Android)
				{
					uri = $"/{uri}/Nav/";
				}
				uri += $"{menu.PageKey}";
				await NavigationService.NavigateAsync(uri, parameters: new NavigationParameters { { "currentPage", menu } }, useModalNavigation: false);
			}
		}

		public override void OnNavigatingTo(INavigationParameters parameters)
		{
			if (parameters.ContainsKey("selectedMenu"))
				SelectedItem = Items.SingleOrDefault(m => m.MenuTitle == (string)parameters["selectedMenu"]) 
					       ?? Items.First();
			else
			{
				var paths = NavigationService.GetNavigationUriPath().Skip(1).ToString().Split('/');
				SelectedItem = Items.SingleOrDefault(i => i.PageKey == paths.Last()) 
					       ?? Items.First();
			}
			//try
			//{
			//	await BuildConfig(_FileStorage);

			//	_ConfigValue = JsonConvert.SerializeObject(Configuration);
			//	await _PageDialogSvc.DisplayAlertAsync("error", _ConfigValue, "ok", "cancel");
			//}
			//catch (System.Exception e)
			//{
			//	await _PageDialogSvc.DisplayAlertAsync("error", e.Message, "", "");
			//}
		}
	}
}
