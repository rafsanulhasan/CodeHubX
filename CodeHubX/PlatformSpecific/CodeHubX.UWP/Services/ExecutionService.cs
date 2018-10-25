using CodeHubX.Models;
using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.ViewModels;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.UI.Core;
using static CodeHubX.Helpers.GlobalHelper;
using static CodeHubX.UWP.Helpers.OctokitNotificationHelper;
using static CodeHubX.UWP.Helpers.ValueSetHelper;
using BadgeHelper = CodeHubX.UWP.Helpers.BadgeHelper;
using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;
using NotificationsService = CodeHubX.Services.NotificationsService;
using NotificationsViewmodel = CodeHubX.UWP.ViewModels.NotificationsViewmodel;
using TilesHelper = CodeHubX.UWP.Helpers.TilesHelper;
using ToastHelper = CodeHubX.UWP.Helpers.ToastHelper;
using ValueSet = Windows.Foundation.Collections.ValueSet;

namespace CodeHubX.UWP.Services
{
	internal static class ExecutionService
	{
		private static readonly CoreDispatcher Dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

		private static async void SendMessage<T>(T messageType)
		    where T : MessageTypeBase, new()
		{
			try
			{
				if (messageType is UpdateUnreadNotificationsCountMessageType uMsgType)
				{
					await RunActionInCoreWindow<BackgroundTaskDeferral>(handler: () =>
					{
						Messenger.Default?.Send(uMsgType);
					});
				}
				if (messageType is UpdateParticipatingNotificationsCountMessageType pMsgType)
				{
					await RunActionInCoreWindow<BackgroundTaskDeferral>(() =>
					{
						Messenger.Default?.Send(pMsgType);
					});
				}
				if (messageType is UpdateAllNotificationsCountMessageType aMsgType)
				{
					await RunActionInCoreWindow<BackgroundTaskDeferral>(() =>
					{
						Messenger.Default?.Send(aMsgType);
					});
				}
			}
			catch (Exception ex)
			{
				ToastHelper.ShowMessage(ex.Message, ex.ToString());
			}
		}

		private static async Task Execute(QueryString args) => await Execute(args.ToBackgroudTaskArgument());

		private static async Task Execute(ValueSet args) => await Execute(args.ToBackgroudTaskArgument());

		private static async Task Execute(BackgroundTaskArgument args)
		{
			try
			{
				switch (args.Action)
				{
					case "sync":
						if (args.What == "notifications")
						{
							var filter = args.Filter;
							bool isAll = filter == "all",
								isParticipating = filter == "participating",
								isUnread = filter == "unread";

							if (isAll)
							{
								NotificationsViewmodel.AllNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(true, false);
							}
							else if (isParticipating)
							{
								NotificationsViewmodel.ParticipatingNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, true);
							}
							else if (isUnread)
							{
								AppViewmodel.UnreadNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, false);
							}

							if (args.WillSendMessage)
							{
								if (isAll)
								{
									SendMessage(new UpdateAllNotificationsCountMessageType { Count = NotificationsViewmodel.AllNotifications?.Count ?? 0 });
								}
								else if (isParticipating)
								{
									SendMessage(new UpdateParticipatingNotificationsCountMessageType { Count = NotificationsViewmodel.ParticipatingNotifications?.Count ?? 0 });
								}
								else if (isUnread)
								{
									SendMessage(new UpdateUnreadNotificationsCountMessageType { Count = AppViewmodel.UnreadNotifications?.Count ?? 0 });
								}
							}

							if (SettingsService.Get<bool>(SettingsKeys.IsToastEnabled))
							{
								await AppViewmodel.UnreadNotifications?.ShowToasts();
							}

							if (SettingsService.Get<bool>(SettingsKeys.IsLiveTilesEnabled))
							{
								await TilesHelper.UpdateTile(AppViewmodel.UnreadNotifications[0]);
							}

							if (SettingsService.Get<bool>(SettingsKeys.IsLiveTilesBadgeEnabled))
							{
								BadgeHelper.UpdateBadge(AppViewmodel.UnreadNotifications?.Count ?? 0);
							}
						}
						break;
					case "show":
						if (args.What == "notifications")
						{
							if (args.Where == "toast")
							{
								await AppViewmodel.UnreadNotifications?.ShowToasts();
							}
							else
							{
								await TilesHelper.UpdateTile(
										  AppViewmodel.UnreadNotifications[0]);
								if (args.WillUpdateBadge)
								{
									BadgeHelper.UpdateBadge(AppViewmodel.UnreadNotifications?.Count ?? 0);
								}
							}
						}
						break;
					case "mark":
						if (args.What == "notifications")
						{
							await NotificationsService.MarkAllNotificationsAsRead();
						}
						else if (args.What == "notification")
						{
							await NotificationsService.MarkNotificationAsRead(args.NotificationId);
							//AppViewmodel.UnreadNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, false);
							//var count = AppViewmodel.UnreadNotifications?.Count ?? 0;
							//if (args.WillSendMessage)
							//{
							//    Messenger.Default.Send(new UpdateUnreadNotificationsCountMessageType
							//    {
							//        Count = count
							//    });
							//}
							//if (args.WillUpdateBadge)
							//{
							//    BadgeHelper.UpdateBadge(count);
							//}
							//await AppViewmodel.UnreadNotifications?.ShowToasts();
						}
						break;
				}
			}
			catch (Exception ex)
			{
				ToastHelper.ShowMessage(ex.Message, ex.ToString());
			}
		}

		public static async Task RunActionAsExtentedAction<TDeferral>(this ExtendedExecutionSession session, Action action, TypedEventHandler<object, ExtendedExecutionRevokedEventArgs> revoked, TDeferral deferral = null)
		    where TDeferral : class
		{
			if (session is null)
			{
				throw new NullReferenceException($"'{nameof(session)} can not be null'");
			}
			var result = await session.RequestExtensionAsync();
			if (result == ExtendedExecutionResult.Allowed)
			{
				action();
			}
			session.Revoked -= revoked;
			session.Dispose();
			session = null;

			if (deferral is AppServiceDeferral aDeferral)
			{
				aDeferral?.Complete();
			}
			else if (deferral is BackgroundTaskDeferral bDeferral)
			{
				bDeferral?.Complete();
			}
		}

		public static async Task RunActionAsExtentedAction<TDeferral>(this ExtendedExecutionSession session, BackgroundTaskArgument
		    args, TypedEventHandler<object, ExtendedExecutionRevokedEventArgs> revoked, TDeferral deferral = null)
		   where TDeferral : class
		{
			if (session is null)
			{
				throw new NullReferenceException($"'{nameof(session)} can not be null'");
			}
			var result = await session.RequestExtensionAsync();
			if (result == ExtendedExecutionResult.Allowed)
			{
				await Execute(args);
			}
			session.Revoked -= revoked;
			session.Dispose();
			session = null;

			if (deferral is AppServiceDeferral aDeferral)
			{
				aDeferral?.Complete();
			}
			else if (deferral is BackgroundTaskDeferral bDeferral)
			{
				bDeferral?.Complete();
			}
		}

		public static async Task RunActionInCoreWindow<TDeferral>(DispatchedHandler handler, TDeferral deferral = null)
		    where TDeferral : class
		{
			if (Dispatcher.HasThreadAccess)
			{
				handler.Invoke();
			}
			else
			{
				await Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, handler);
			}

			if (deferral is AppServiceDeferral aDeferral)
			{
				aDeferral?.Complete();
			}
			else if (deferral is BackgroundTaskDeferral bDeferral)
			{
				bDeferral?.Complete();
			}
		}

		public static async Task RunActionInCoreWindow<TDeferral>(BackgroundTaskArgument args, TDeferral deferral = null)
		    where TDeferral : class
		{
			if (Dispatcher.HasThreadAccess)
			{
				await Execute(args);
			}
			else
			{
				await Dispatcher.AwaitableRunAsync(
				    () => Execute(args),
				    CoreDispatcherPriority.Normal);
			}
			if (deferral is AppServiceDeferral aDeferral)
			{
				aDeferral?.Complete();
			}
			else if (deferral is BackgroundTaskDeferral bDeferral)
			{
				bDeferral?.Complete();
			}
		}

		public static async Task RunActionInUiThread<TDeferral>(Action action, TDeferral deferral = null)
		    where TDeferral : class
		{
			if (Dispatcher.HasThreadAccess)
			{
				action();
			}
			else
			{
				await DispatcherHelper.ExecuteOnUIThreadAsync(action, CoreDispatcherPriority.Normal);
			}
			if (deferral is AppServiceDeferral aDeferral)
			{
				aDeferral?.Complete();
			}
			else if (deferral is BackgroundTaskDeferral bDeferral)
			{
				bDeferral?.Complete();
			}
		}

		public static async Task RunActionInUiThread<TDeferral>(BackgroundTaskArgument args, TDeferral deferral = null)
		    where TDeferral : class
		{
			if (Dispatcher.HasThreadAccess)
			{
				await Execute(args);
			}
			else
			{
				await DispatcherHelper.ExecuteOnUIThreadAsync(
				    () => Execute(args),
				    CoreDispatcherPriority.Normal
				);
			}
			if (deferral is AppServiceDeferral aDeferral)
			{
				aDeferral?.Complete();
			}
			else if (deferral is BackgroundTaskDeferral bDeferral)
			{
				bDeferral?.Complete();
			}
		}
	}
}
