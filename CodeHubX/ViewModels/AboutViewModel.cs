
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;

using Xamarin.Forms;

namespace CodeHubX.ViewModels
{
	public class AboutViewModel : ViewModelBase
	{
		public DelegateCommand OpenWebCommand { get; set; }

		public AboutViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
			: base(navigationService, pageDialogService)
		{
			Title = "About";

			OpenWebCommand = new DelegateCommand(()
				=> Device.OpenUri(new Uri("https://xamarin.com/platform")));
		}
	}
}