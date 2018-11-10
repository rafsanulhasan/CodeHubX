using CodeHubX.Services;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using static CodeHubX.Helpers.GlobalHelper;
using Task = System.Threading.Tasks.Task;

namespace CodeHubX.ViewModels
{
	public class NotificationsViewmodel : AppViewmodel
	{
		#region properties
		public static ObservableCollection<Notification> AllNotifications { get; set; }

		public static ObservableCollection<Notification> ParticipatingNotifications { get; set; }

		public bool _ZeroAllCount;
		public bool ZeroAllCount
		{
			get => _ZeroAllCount;
			set => Set(() => ZeroAllCount, ref _ZeroAllCount, value);
		}
		public bool _ZeroUnreadCount;
		public bool ZeroUnreadCount
		{
			get => _ZeroUnreadCount;
			set => Set(() => ZeroUnreadCount, ref _ZeroUnreadCount, value);
		}
		public bool _ZeroParticipatingCount;
		public bool ZeroParticipatingCount
		{
			get => _ZeroParticipatingCount;
			set => Set(() => ZeroParticipatingCount, ref _ZeroParticipatingCount, value);
		}

		public bool _isloadingAll;
		public bool IsLoadingAll
		{
			get => _isloadingAll;
			set => Set(() => IsLoadingAll, ref _isloadingAll, value);
		}

		public bool _isloadingUnread;
		public bool IsLoadingUnread
		{
			get => _isloadingUnread;
			set => Set(() => IsLoadingUnread, ref _isloadingUnread, value);
		}

		public bool _isloadingParticipating;
		public bool IsloadingParticipating
		{
			get => _isloadingParticipating;
			set => Set(() => IsloadingParticipating, ref _isloadingParticipating, value);
		}
		#endregion

		public NotificationsViewmodel()
		{
			UnreadNotifications = UnreadNotifications ?? new ObservableCollection<Notification>();
			AllNotifications = AllNotifications ?? new ObservableCollection<Notification>();
			ParticipatingNotifications = ParticipatingNotifications ?? new ObservableCollection<Notification>();
		}

		public async Task Load()
		{
			if (!IsInternet())
			{
				//Sending NoInternet message to all viewModels
				//Messenger.Default.Send(new NoInternet().SendMessage());
				MessagingCenter.Instance.Send(this, null, new NoInternet().SendMessage());
			}
			else
			{
				IsLoadingUnread = true;
				await LoadUnreadNotifications();
				IsLoadingUnread = false;
				IsLoadingAll = true;
				await LoadAllNotifications();
				IsLoadingAll = false;
				IsloadingParticipating = true;
				await LoadParticipatingNotifications();
				IsloadingParticipating = false;
			}
		}

		public async void RefreshAll()
		{
			if (!IsInternet())
			{
				//Sending NoInternet message to all viewModels
				//Messenger.Default.Send(new NoInternet().SendMessage());
				MessagingCenter.Instance.Send(this, null, new NoInternet().SendMessage());
			}
			else
			{
				IsLoadingAll = true;
				await LoadAllNotifications();
				IsLoadingAll = false;
			}
		}
		public async void RefreshUnread()
		{
			if (!IsInternet())
			{
				//Sending NoInternet message to all viewModels
				//Messenger.Default.Send(new NoInternet().SendMessage());
				MessagingCenter.Instance.Send(this, null, new NoInternet().SendMessage());
			}
			else
			{
				IsLoadingUnread = true;
				await LoadUnreadNotifications();
				IsLoadingUnread = false;
			}
		}
		public async void RefreshParticipating()
		{

			if (!IsInternet())
			{
				//Sending NoInternet message to all viewModels
				//Messenger.Default.Send(new NoInternet().SendMessage());
				MessagingCenter.Instance.Send(this, null, new NoInternet().SendMessage());
			}
			else
			{
				IsloadingParticipating = true;
				await LoadParticipatingNotifications();
				IsloadingParticipating = false;
			}
		}

		public async void MarkAllNotificationsAsRead()
		{
			if (!IsInternet())
			{
				//Sending NoInternet message to all viewModels
				//Messenger.Default.Send(new NoInternet().SendMessage());
				MessagingCenter.Instance.Send(this, null, new NoInternet().SendMessage());
			}
			else
			{
				IsLoadingAll = IsLoadingUnread = IsloadingParticipating = true;
				await NotificationsService.MarkAllNotificationsAsRead();
				IsLoadingAll = IsLoadingUnread = IsloadingParticipating = false;
				Messenger.Default.Send(new UpdateAllNotificationsCountMessageType
				{
					Count = 0
				});
			}
		}

		public void RecieveSignOutMessage(SignOutMessageType empty)
		{
			IsLoggedin = false;
			User = null;
			AllNotifications = UnreadNotifications = ParticipatingNotifications = null;
		}

		public async void RecieveSignInMessage(User user)
		{
			if (user != null)
			{
				IsLoggedin = true;
				User = user;
				await Load();
			}
		}

		private async Task LoadAllNotifications()
			=> AllNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(true, false);

		private async Task LoadUnreadNotifications()
			=> UnreadNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, false);

		private async Task LoadParticipatingNotifications()
			=> ParticipatingNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, true);

		public async void Pivot_SelectionChanged(object sender, EventHandler e)
		{
			//if (p.SelectedItem == typeof(Unread))
			//{
			//	IsLoadingUnread = true;
			//	await LoadUnreadNotifications();
			//	IsLoadingUnread = false;
			//}
			//else if (p.SelectedIndex == 1)
			//{
			//	IsloadingParticipating = true;
			//	await LoadParticipatingNotifications();
			//	IsloadingParticipating = false;
			//}
			//else if (p.SelectedIndex == 2)
			//{
			//	IsLoadingAll = true;
			//	await LoadAllNotifications();
			//	IsLoadingAll = false;
			//}
		}

		public async void NotificationsListView_ItemClick(object sender, SelectedItemChangedEventArgs e)
		{
			var notif = e.SelectedItem as Notification;
			var isIssue = notif.Subject.Type.ToLower().Equals("issue");
			Issue issue = null;
			PullRequest pr = null;
			if (isIssue)
			{
				if (int.TryParse(notif.Subject.Url.Split('/').Last().Split('?')[0], out var id))
				{
					issue = await IssueUtility.GetIssue(notif.Repository.Id, id);
					//await DependencyService.Resolve<IAsyncNavigationService>().NavigateAsync(typeof(IssueDetailView), new System.Tuple<Repository, Issue>(notif.Repository, issue));
				}
			}
			else
			{

				if (int.TryParse(notif.Subject.Url.Split('/').Last().Split('?')[0], out var id))
				{
					pr = await PullRequestUtility.GetPullRequest(notif.Repository.Id, id);
					//await DependencyService.Resolve<IAsyncNavigationService>().NavigateAsync(typeof(PullRequestDetailView), new System.Tuple<Repository, PullRequest>(notif.Repository, pr));
				}
			}
			if (notif.Unread)
			{
				await NotificationsService.MarkNotificationAsRead(notif.Id);
				//await BackgroundTaskService.SyncUnreadNotifications(sendMessage: true);
				//await BackgroundTaskService.ShowNotifications("toast");
				//await BackgroundTaskService.ShowNotifications("tiles");
			}
		}
	}
}
