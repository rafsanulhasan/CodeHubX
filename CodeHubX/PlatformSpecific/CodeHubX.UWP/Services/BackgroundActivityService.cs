using CodeHubX.Models;
using CodeHubX.Services;
using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.QueryStringDotNET;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.System.Threading;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using static CodeHubX.Helpers.GlobalHelper;
using ToastHelper = CodeHubX.UWP.Helpers.ToastHelper;

namespace CodeHubX.UWP.Services
{
	internal class BackgroundActivityService
	{
		private BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
		private bool _cancelRequested = false;
		private BackgroundTaskDeferral _deferral = null;
		private ThreadPoolTimer _periodicTimer = null;
		private uint _progress = 0;
		private IBackgroundTaskInstance _taskInstance = null;

		private ExtendedExecutionSession _ExExecSession;
		private BackgroundTaskDeferral _AppTriggerDeferral;
		private BackgroundTaskDeferral _SyncDeferral;
		private BackgroundTaskDeferral _ToastActionDeferral;
		public static Window Window { get; internal set; }

		private void ExExecSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
		{
			_ExExecSession.Dispose();
			_ExExecSession = null;
		}

		private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			_cancelRequested = true;
			_cancelReason = reason;

			Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
			switch (sender.Task.Name)
			{
				case "AppTrigger":
					_AppTriggerDeferral?.Complete();
					break;
				case "SyncNotifications":
					_SyncDeferral?.Complete();
					break;
				case "ToastNotificationAction":
					_ToastActionDeferral?.Complete();
					break;
			}
			//_ExExecSession.Revoked -= ExExecSession_Revoked;
			_ExExecSession?.Dispose();
			_ExExecSession = null;
			ToastHelper.ShowMessage($"{sender.Task.Name} has been canceled", reason.ToString());
		}

		public BackgroundActivityService()
		{
			_ExExecSession = new ExtendedExecutionSession();
			_ExExecSession.Revoked += ExExecSession_Revoked;
		}

		public BackgroundActivityService(Window window)
			: this() => Window = window;

		//
		// The Run method is the entry point of a background task.
		//
		public async void Run(BackgroundActivatedEventArgs args)
		{
			_taskInstance = args.TaskInstance;
			Debug.WriteLine($"Background {_taskInstance.Task.Name} Starting...");

			_taskInstance.Canceled += TaskInstance_Canceled;
			var triggerDetails = _taskInstance.TriggerDetails;
			var taskName = _taskInstance.Task.Name;
			switch (taskName)
			{
				case "AppTrigger":
					_AppTriggerDeferral = _taskInstance.GetDeferral();

					if (!(triggerDetails is ApplicationTriggerDetails appTriggerDetails))
					{
						throw new InvalidOperationException("Task requires trigger to be valid ApplicationTrigger");
					}

					var appTriggerArgs = BackgroundTaskArgumentHelper.Parse(appTriggerDetails.Arguments);

					await _ExExecSession.RunActionAsExtentedAction(async () =>
					{
						await ExecutionService.RunActionInUiThread<BackgroundTaskDeferral>(appTriggerArgs);
					}, ExExecSession_Revoked, _AppTriggerDeferral);
					break;

				case "SyncNotifications":
					_SyncDeferral = _taskInstance.GetDeferral();

					await _ExExecSession.RunActionAsExtentedAction(async () =>
					{
						await ExecutionService.RunActionInUiThread<BackgroundTaskDeferral>(async () =>
					{
						try
						{
							await BackgroundTaskService.SyncUnreadNotifications(sendMessage: Window.Content != null);
							await BackgroundTaskService.ShowNotifications("toast");
							await BackgroundTaskService.ShowNotifications("tiles");
						}
						catch (Exception ex)
						{
							ToastHelper.ShowMessage(ex.Message, ex.InnerException?.Message ?? ex.StackTrace ?? ex.ToString());
						}
					});
					}, ExExecSession_Revoked, _SyncDeferral);
					break;

				case "ToastNotificationAction":
					_ToastActionDeferral = _taskInstance.GetDeferral();

					if (!(triggerDetails is ToastNotificationActionTriggerDetail toastTriggerDetails))
					{
						throw new ArgumentException("Task requires trigger to be valid ToastNotificationActionTrigger");
					}

					await _ExExecSession.RunActionAsExtentedAction(async () =>
					{
						await ExecutionService.RunActionInCoreWindow<BackgroundTaskDeferral>(async () =>
					{
						try
						{
							var toastArgs = BackgroundTaskArgumentHelper.Parse(QueryString.Parse(toastTriggerDetails.Argument));
							await NotificationsService.MarkNotificationAsRead(toastArgs.NotificationId);
							AppViewmodel.UnreadNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, false);
							var count = AppViewmodel.UnreadNotifications?.Count ?? 0;
							if (toastArgs.WillSendMessage)
							{
								Messenger.Default.Send(new UpdateUnreadNotificationsCountMessageType
								{
									Count = count
								});
							}
							if (toastArgs.WillUpdateBadge)
							{
								BadgeHelper.UpdateBadge(count);
							}
							//await AppViewmodel.UnreadNotifications?.ShowToasts();
						}
						catch (Exception ex)
						{
							ToastHelper.ShowMessage(ex.Message, ex.InnerException?.Message ?? ex.ToString());
						}
					});
					}, ExExecSession_Revoked, _ToastActionDeferral);

					break;
			}

			//
			// Query BackgroundWorkCost
			// Guidance: If BackgroundWorkCost is high, then perform only the minimum amount
			// of work in the background task and return immediately.
			//
			var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;

			//
			// Associate a cancellation handler with the background task.
			//
			//_taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

			//
			// Get the deferral object from the task instance, and take a reference to the taskInstance;
			//
			//_deferral = _taskInstance.GetDeferral();

			//_periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(1));
		}

		//
		// Handles background task cancellation.
		//
		private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			//
			// Indicate that the background task is canceled.
			//
			_cancelRequested = true;
			_cancelReason = reason;

			Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
		}

		//
		// Simulate the background task activity.
		//
		private void PeriodicTimerCallback(ThreadPoolTimer timer)
		{
			if ((_cancelRequested == false) && (_progress < 100))
			{
				_progress += 10;
				_taskInstance.Progress = _progress;
			}
			else
			{
				_periodicTimer.Cancel();

				var key = _taskInstance.Task.Name;

				//
				// Record that this background task ran.
				//
				var taskStatus = (_progress < 100) ? "Canceled with reason: " + _cancelReason.ToString() : "Completed";
				//BackgroundTaskSample.TaskStatuses[key] = taskStatus;
				Debug.WriteLine("Background " + _taskInstance.Task.Name + taskStatus);

				//
				// Indicate that the background task has completed.
				//
				_deferral.Complete();
			}
		}

		public static void Start(BackgroundTaskRegistrationGroup sender, BackgroundActivatedEventArgs args)
		{
			// Use the taskInstance.Name and/or taskInstance.InstanceId to determine
			// what background activity to perform. In this sample, all of our
			// background activities are the same, so there is nothing to check.
			var activity = new BackgroundActivityService();
			activity.Run(args);
		}

		public static void Start(BackgroundTaskRegistrationGroup sender, BackgroundActivatedEventArgs args, Window window)
		{
			// Use the taskInstance.Name and/or taskInstance.InstanceId to determine
			// what background activity to perform. In this sample, all of our
			// background activities are the same, so there is nothing to check.
			var activity = new BackgroundActivityService(window);
			activity.Run(args);
		}
	}
}
