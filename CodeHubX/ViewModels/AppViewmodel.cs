﻿using CodeHubX.Helpers;
using CodeHubX.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeHubX.ViewModels
{
	public class AppViewmodel : ViewModelBase
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

		public AppViewmodel()
		{
			UnreadNotifications = UnreadNotifications ?? new ObservableCollection<Notification>();
			ToolbarItems = new List<ToolbarItem>()
			{
				new ToolbarItem("Notifications", "notifications", ()=>{ }, ToolbarItemOrder.Primary),
				new ToolbarItem("Refresh", "refresh", ()=>{ }, ToolbarItemOrder.Secondary)
			};
		}

		public async Task ConfigureAdsVisibility()
		{
			if (await HasAlreadyDonated())
			{
				GlobalHelper.HasAlreadyDonated = true;
				ToggleAdsVisiblity();
			}
			else
			{
				SettingsService.Save(SettingsKeys.IsAdsEnabled, true);

				//if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
				//{
				//	IsMobileAdsVisible = true;
				//	IsDesktopAdsVisible = false;
				//}
				//else
				//{
				//	IsDesktopAdsVisible = true;
				//	IsMobileAdsVisible = false;
				//}
			}
		}
		//public async void Error(Exception ex) 
		//	=> await new MessageDialog(ex.ToString(), ex.Message).ShowAsync();
		//public async void Error(string message, string title = "Unknown Error") 
		//	=> await new MessageDialog(message, title).ShowAsync();

		//public async void MarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
		//{
		//try
		//{
		//	await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
		//}
		//catch (UriFormatException) { await new MessageDialog("Incorrect URI Format").ShowAsync(); }
		//}

		public async Task Navigate(Type pageType)
			=> await DependencyService.Resolve<IAsyncNavigationService>().NavigateAsync(pageType, User);

		public async Task Navigate<TType, TViewModel>(TType pageType, TViewModel viewModel)
		    where TType : Xamarin.Forms.Page
		    where TViewModel : AppViewmodel
			=> await Navigate(typeof(TType), viewModel);

		public async Task Navigate<TViewModel>(Type pageType, TViewModel viewModel)
		    where TViewModel : AppViewmodel
			=> await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(pageType, viewModel);

		public async Task GoBack()
			=> await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().GoBackAsync();

		public async Task<bool> HasAlreadyDonated()
		{
			try
			{
				if (SettingsService.Get<bool>(SettingsKeys.HasUserDonated))
					return true;
				else
				{
					//var WindowsStore = StoreContext.GetDefault();

					//var productKinds = new[] { "Durable" };
					//var filterList = new List<string>(productKinds);

					//var queryResult = await WindowsStore.GetUserCollectionAsync(filterList);

					//if (queryResult.ExtendedError != null)
					//	return false;

					//foreach (var item in queryResult.Products)
					//{
					//	if (item.Value != null)
					//	{
					//		if (item.Value.IsInUserCollection)
					//		{
					//			SettingsService.Save(SettingsKeys.HasUserDonated, true, true);
					//			return true;
					//		}
					//	}
					//	return false;
					//}
				}

				return false;
			}
			catch { return false; }
		}

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
				//if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
				//{
				//	IsMobileAdsVisible = true;
				//	IsDesktopAdsVisible = false;
				//}
				//else
				//{
				//	IsDesktopAdsVisible = true;
				//	IsMobileAdsVisible = false;
				//}
			}
			else
				IsMobileAdsVisible = IsDesktopAdsVisible = false;
		}
	}
}
