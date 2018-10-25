using CodeHubX.Helpers;
using CodeHubX.Models;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace CodeHubX.UWP.Services
{
	internal static class BackgroundTaskService
	{
		#region private members
		#region Fields
		private static ApplicationTrigger AppTrigger;
		private static ApplicationTriggerResult _allResult;
		private static ApplicationTriggerResult _participatingResult;
		private static ApplicationTriggerResult _unreadResult;
		private static ApplicationTriggerResult _showToastResult;
		private static ApplicationTriggerResult _showTilesResult;
		private static ApplicationTriggerResult _markResult;
		private static ApplicationTriggerResult _markAllResult;
		#endregion

		#region methods

		private static void ResetAppDataForAppTriggertask()
			=> ApplicationData.Current.LocalSettings.Values.Remove("AppTrigger");

		private static async Task<ApplicationTriggerResult> RunAppTrigger(BackgroundTaskArgument args, bool resetAppData = true)
		{
			if (resetAppData)
			{
				ResetAppDataForAppTriggertask();
			}

			return await GetAppTrigger()?.RequestAsync(args.To<ValueSet>());
		}

		private static async Task<ApplicationTriggerResult> RunAppTrigger(ValueSet valueSet, bool resetAppData = true)
		{
			if (resetAppData)
			{
				ResetAppDataForAppTriggertask();
			}

			return await GetAppTrigger()?.RequestAsync(valueSet);
		}
		#endregion
		#endregion

		#region Internal Members
		#region Methods   
		internal static async Task<ApplicationTriggerResult> RunAppTriggerBackgroundTask(BackgroundTaskArgument args, bool resetAppData = true)
		{
			var valueSet = new ValueSet
			{
				{ nameof(args.Action), args.Action},
				{ nameof(args.What) , args.What },
				{ nameof(args.Where) , args.Where },
				{ nameof(args.Filter),  args.Filter },
				{ nameof(BackgroundTaskArgument.WillSendMessage),  args.WillSendMessage},
				{ nameof(BackgroundTaskArgument.IsGhost),  args.IsGhost },
				{ nameof(BackgroundTaskArgument.WillUpdateBadge), args.WillUpdateBadge }
			};

			return await RunAppTrigger(valueSet, resetAppData);
		}


		internal static async Task<ApplicationTriggerResult> RunAppTriggerBackgroundTask(ValueSet args, bool resetAppData = true) 
			=> await RunAppTrigger(args, resetAppData);
		#endregion
		#endregion

		#region Public Members
		#region Methods

		public static ApplicationTrigger GetAppTrigger()
		{
			if (AppTrigger == null)
			{
				AppTrigger = new ApplicationTrigger();
			}

			return AppTrigger;
		}

		public static async Task MarkNotificationAsRead(string notificationId, bool sendMessage = false, bool reset = true, BackgroundTaskDeferral deferral = null)
		{
			reset = _markResult == ApplicationTriggerResult.Allowed
				    && _markResult == ApplicationTriggerResult.CurrentlyRunning;
			if (_markResult != ApplicationTriggerResult.CurrentlyRunning)
			{
				var args = new BackgroundTaskArgument
				{
					Action = "mark",
					What = "notification",
					NotificationId = notificationId,
					WillSendMessage = sendMessage
				};
				_markResult = await RunAppTriggerBackgroundTask(args, reset);
			}

			deferral?.Complete();
		}

		public static async Task MarkNotificationsAsRead(bool sendMessage = false, bool reset = true, BackgroundTaskDeferral deferral = null)
		{
			reset = _markAllResult == ApplicationTriggerResult.Allowed
				    && _markAllResult == ApplicationTriggerResult.CurrentlyRunning;
			if (_markAllResult != ApplicationTriggerResult.CurrentlyRunning)
			{
				var args = new BackgroundTaskArgument
				{
					Action = "mark",
					What = "notifications",
					WillSendMessage = sendMessage
				};
				_markAllResult = await RunAppTriggerBackgroundTask(args, reset);
			}

			deferral?.Complete();
		}

		public static async Task ShowNotifications(string where, bool updateBadge = true, bool ghost = false, bool reset = true, BackgroundTaskDeferral deferral = null)
		{
			reset = _showToastResult == ApplicationTriggerResult.Allowed
				    && _showToastResult == ApplicationTriggerResult.CurrentlyRunning;
			var args = new BackgroundTaskArgument
			{
				Action = "show",
				What = "notifications",
				Where = where,
				WillUpdateBadge = updateBadge,
				IsGhost = ghost
			};
			if (where == "toast")
			{
				if (_showToastResult != ApplicationTriggerResult.CurrentlyRunning)
				{
					_showToastResult = await RunAppTriggerBackgroundTask(args, reset);
				}
			}
			else if (where == "tiles")
			{

				if (_showTilesResult != ApplicationTriggerResult.CurrentlyRunning)
				{
					_showTilesResult = await RunAppTriggerBackgroundTask(args, reset);
				}
			}

			deferral?.Complete();
		}

		public static async Task SyncAllNotifications(bool sendMessage = false, bool reset = true, BackgroundTaskDeferral deferral = null)
		{
			reset = _allResult == ApplicationTriggerResult.Allowed
				    && _allResult == ApplicationTriggerResult.CurrentlyRunning;
			if (_allResult != ApplicationTriggerResult.CurrentlyRunning)
			{
				var args = new BackgroundTaskArgument
				{
					Action = "sync",
					What = "notifications",
					Filter = "all",
					WillSendMessage = sendMessage
				};
				_allResult = await RunAppTriggerBackgroundTask(args, reset);
			}

			deferral?.Complete();
		}

		public static async Task SyncParticipatingNotifications(bool sendMessage = false, bool reset = true, BackgroundTaskDeferral deferral = null)
		{
			reset = _participatingResult == ApplicationTriggerResult.Allowed
				    && _participatingResult == ApplicationTriggerResult.CurrentlyRunning;
			if (_participatingResult != ApplicationTriggerResult.CurrentlyRunning)
			{
				var args = new BackgroundTaskArgument
				{
					Action = "sync",
					What = "notifications",
					Filter = "participating",
					WillSendMessage = sendMessage
				};
				_participatingResult = await RunAppTriggerBackgroundTask(args, reset);
			}

			deferral?.Complete();
		}

		public static async Task SyncUnreadNotifications(bool sendMessage = false, bool reset = true, BackgroundTaskDeferral deferral = null)
		{
			reset = _unreadResult == ApplicationTriggerResult.Allowed
				    && _unreadResult == ApplicationTriggerResult.CurrentlyRunning;
			if (_unreadResult != ApplicationTriggerResult.CurrentlyRunning)
			{
				var args = new BackgroundTaskArgument
				{
					Action = "sync",
					What = "notifications",
					Filter = "unread",
					WillSendMessage = sendMessage
				};
				_unreadResult = await RunAppTriggerBackgroundTask(args, reset);
			}

			deferral?.Complete();
		}
		#endregion
		#endregion
	}
}
