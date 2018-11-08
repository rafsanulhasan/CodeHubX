using CodeHubX.Helpers;
using CodeHubX.Services;
using CodeHubX.Views;
using GalaSoft.MvvmLight;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeHubX.ViewModels
{
	public partial class AppViewmodel : ViewModelBase
	{
		#region properties
		private bool _isLoggedin;
		public bool IsLoggedin
		{
			get => _isLoggedin;
			set => Set(() => IsLoggedin, ref _isLoggedin, value);
		}

		private bool _isLoading;
		public bool IsLoading
		{
			get => _isLoading;
			set => Set(() => IsLoading, ref _isLoading, value);
		}

		private User _user;
		public User User
		{
			get => _user;
			set => Set(() => User, ref _user, value);
		}

		private bool _IsNotificationsUnread;
		public bool IsNotificationsUnread
		{
			get => _IsNotificationsUnread;
			set => Set(() => IsNotificationsUnread, ref _IsNotificationsUnread, value);
		}

		private int _NumberOfAllNotifications;
		public int NumberOfAllNotifications
		{
			get => _NumberOfAllNotifications;
			set => Set(() => NumberOfAllNotifications, ref _NumberOfAllNotifications, value);
		}

		private int _NumberOfParticipatingNotifications;
		public int NumberOfParticipatingNotifications
		{
			get => _NumberOfParticipatingNotifications;
			set => Set(() => NumberOfParticipatingNotifications, ref _NumberOfParticipatingNotifications, value);
		}

		private int _NumberOfUnreadNotifications;
		public int NumberOfUnreadNotifications
		{
			get => _NumberOfUnreadNotifications;
			set => Set(() => NumberOfUnreadNotifications, ref _NumberOfUnreadNotifications, value);
		}

		private bool _isDesktopAdsVisible;
		public bool IsDesktopAdsVisible
		{
			get => _isDesktopAdsVisible;
			set => Set(() => IsDesktopAdsVisible, ref _isDesktopAdsVisible, value);
		}

		private bool _isMobileAdsVisible;
		public bool IsMobileAdsVisible
		{
			get => _isMobileAdsVisible;
			set => Set(() => IsMobileAdsVisible, ref _isMobileAdsVisible, value);
		}

		public string WhatsNewText
			 => "Hi all! \nHere's the changelog for v2.4.14\n\n\x2022 Added Turkish translations\n\x2022 Minor fluent UI improvements\n\x2022 Target build updated to 17134.0\n\x2022 Clicking on notifications now lands you on the specific issue or PR  \n\n NOTE: Please update to Fall creator's update or above to get latest CodeHub updates.";

		private string _AllString = $" ({NotificationsViewmodel.AllNotifications?.Count ?? 0})";
		public string AllString
		{
			get => _AllString;
			set => Set(() => AllString, ref _AllString, value);
		}

		private string _ParticipatingString = $" ({NotificationsViewmodel.ParticipatingNotifications?.Count ?? 0})";
		public string ParticipatingString
		{
			get => _ParticipatingString;
			set => Set(() => ParticipatingString, ref _ParticipatingString, value);
		}

		private string _UnreadString = $" ({UnreadNotifications?.Count ?? 0})";
		public string UnreadString
		{
			get => _UnreadString;
			protected set => Set(() => UnreadString, ref _UnreadString, value);
		}

		public IList<ToolbarItem> ToolbarItems { get; protected set; }


		public static ObservableCollection<Notification> UnreadNotifications { get; set; }
		#endregion

		private const string donateFirstAddOnId = "9pd0r1dxkt8j";
		private const string donateSecondAddOnId = "9msvqcz4pbws";
		private const string donateThirdAddOnId = "9n571g3nr2cs";
		private const string donateFourthAddOnId = "9nsmgzx3p43x";
		private const string donateFifthAddOnId = "9phrhpvhscdv";
		private const string donateSixthAddOnId = "9nnqdq0kq21j";
		private readonly MainPage _RootPage = Xamarin.Forms.Application.Current.MainPage as MainPage;
		private readonly NavigationPage _CurrentPage = NavigationService.CurrentSourcePage;

		partial void HasAlreadyDonated(ref bool result);

		public AppViewmodel()
		{
			UnreadNotifications = UnreadNotifications ?? new ObservableCollection<Notification>();
			ToolbarItems = new List<ToolbarItem>()
			{
				new ToolbarItem("Notifications", "notifications", ()=>{ }, ToolbarItemOrder.Primary),
				new ToolbarItem("Refresh", "refresh", ()=>{ }, ToolbarItemOrder.Secondary)
			};
		}

		public void ConfigureAdsVisibility()
		{
			var hasAlreadyDonated = false;
			HasAlreadyDonated(ref hasAlreadyDonated);
			if (hasAlreadyDonated)
			{
				GlobalHelper.HasAlreadyDonated = true;
				ToggleAdsVisiblity();
			}
			else
			{
				SettingsService.Save(SettingsKeys.IsAdsEnabled, true);
				
				if (GlobalHelper.CurrentDevicefamily.Contains("Phone"))
				{
					IsMobileAdsVisible = true;
					IsDesktopAdsVisible = false;
				}
				else
				{
					IsDesktopAdsVisible = true;
					IsMobileAdsVisible = false;
				}
			}
		}
		public void Error(Exception ex, bool showInCurrentPage = true)
		{
			string title = $"Error {ex.HResult}",
				  msg = string.Empty;

			if (ex is null)
				msg = "Unknown error";
			else
			{
				if (StringHelper.IsNullOrEmptyOrWhiteSpace(ex.Message))
					msg = ex.StackTrace;
				else
					msg = ex.Message;
			}

			Error(msg, title, showInCurrentPage);
		}

		public async void Error(string message, string title = "Unknown Error", bool showInCurrentPage = true)
		{
			if (showInCurrentPage)
				await _CurrentPage.DisplayAlert(title, message, "Close");
			else
				await _RootPage.DisplayAlert(title, message, "Close");
		}

		//public void MarkdownTextBlock_LinkClicked(object sender, ClickedEventArgs e)
		//{
		//	try
		//	{
		//		Device.OpenUri(MarkdownTextBlock)
		//	}
		//	catch (UriFormatException ufEx)
		//	{
		//		Error(ufEx.Message, "Incorrect URI Format");
		//	}
		//}

		public Task Navigate(CustomContentPage page)
			=> NavigationService.NavigateAsync(page, null);

		public Task Navigate<TViewModel>(CustomContentPage<TViewModel> page, TViewModel viewModel)
		    where TViewModel : AppViewmodel, new()
			=> Navigate(page, viewModel);

		public Task Navigate<TViewModel>(CustomContentPage<TViewModel> pageType, TViewModel viewModel, string title)
		    where TViewModel : AppViewmodel, new()
			=> Navigate(pageType, viewModel);

		public Task GoBack()
			=> NavigationService.GoBackAsync();

		public void UpdateAllNotificationIndicator(int count)
		{
			NumberOfAllNotifications = count;
			AllString = $" ({NumberOfAllNotifications})";
		}

		public void UpdateParticipatingNotificationIndicator(int count)
		{
			NumberOfParticipatingNotifications = count;
			ParticipatingString = $" ({NumberOfParticipatingNotifications})";
		}

		public void UpdateUnreadNotificationIndicator(int count)
		{
			IsNotificationsUnread = count > 0;
			NumberOfUnreadNotifications = count;
			UnreadString = $" ({NumberOfUnreadNotifications})";
		}

		public void ToggleAdsVisiblity()
		{
			if (SettingsService.Get<bool>(SettingsKeys.IsAdsEnabled))
			{
				if (GlobalHelper.CurrentDevicefamily.Contains("Phone"))
				{
					IsMobileAdsVisible = true;
					IsDesktopAdsVisible = false;
				}
				else
				{
					IsDesktopAdsVisible = true;
					IsMobileAdsVisible = false;
				}
			}
			else
				IsMobileAdsVisible = IsDesktopAdsVisible = false;
		}
	}
}
