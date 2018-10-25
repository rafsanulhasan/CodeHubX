using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
	public sealed partial class EditProfileView : Windows.UI.Xaml.Controls.Page
	{
		public EditProfileViewmodel ViewModel;

		public EditProfileView()
		{
			InitializeComponent();
			ViewModel = new EditProfileViewmodel();
			DataContext = ViewModel;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Edit Profile" });
			await ViewModel.Load(e.Parameter);
		}
	}
}
