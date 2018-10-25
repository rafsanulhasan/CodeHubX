using CodeHubX.UWP.Helpers;
using CodeHubX.UWP.Models;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace CodeHubX.UWP.Services
{
	public static class BackgroundTaskRegistrationService
	{
		private static BackgroundTaskRegistrationGroup Group;

		private static void ResetAppDataForAppTriggertask() 
			=> ApplicationData.Current.LocalSettings.Values.Remove("AppTrigger");

		#region Internal members

		public static void RegisterAppBackgroundTasks()
		{
			GetGroup();
			IBackgroundCondition internetAvailableCondition = new SystemCondition(SystemConditionType.InternetAvailable);
			IBackgroundCondition userPresentCondition = new SystemCondition(SystemConditionType.UserPresent);
			IBackgroundCondition sessionConnectedCondition = new SystemCondition(SystemConditionType.SessionConnected);
			IBackgroundCondition backgroundCostNotHighCondition = new SystemCondition(SystemConditionType.BackgroundWorkCostNotHigh);

			var conditions = new[]
			{
				internetAvailableCondition,
                    //userPresentCondition,
                    //sessionConnectedCondition
                };
			var appTrigger = new ApplicationTrigger();
			var bgBuilderModel = new BackgroundTaskBuilderModel(
							 "AppTrigger",
							 BackgroundTaskService.GetAppTrigger(),
							 null,
							 Group.Name,
							 conditions
						   );

			var builder = BackgroundTaskBuilderHelper.BuildTask(bgBuilderModel, true, true);
			try
			{
				ResetAppDataForAppTriggertask();
				BackgroundExecutionManager.RemoveAccess();
			}
			catch
			{

			}
			builder.Register(all: true);
		}

		public static void RegisterBackgroundTasks()
		{
			GetGroup();

			IBackgroundCondition internetAvailableCondition = new SystemCondition(SystemConditionType.InternetAvailable);
			IBackgroundCondition userPresentCondition = new SystemCondition(SystemConditionType.UserPresent);
			IBackgroundCondition sessionConnectedCondition = new SystemCondition(SystemConditionType.SessionConnected);
			IBackgroundCondition backgroundCostNotHighCondition = new SystemCondition(SystemConditionType.BackgroundWorkCostNotHigh);

			var conditions = new[] {
				internetAvailableCondition,
				// userPresentCondition,
				//sessionConnectedCondition
               };

			var bgBuilderModel = new BackgroundTaskBuilderModel(
							    "ToastNotificationAction",
							    new ToastNotificationActionTrigger(),
							    null,
							    null,
							    conditions
							 );
			var toastActionTask = BackgroundTaskBuilderHelper.BuildTask(bgBuilderModel, true, true);
			toastActionTask.Register(true, false, true);

			bgBuilderModel = new BackgroundTaskBuilderModel(
							"SyncNotifications",
							new TimeTrigger(15, false),
							null,
							null,
							conditions
						  );
			var syncTask = BackgroundTaskBuilderHelper.BuildTask(bgBuilderModel, true, true);
			syncTask.Register(true, false, true);
		}
		#endregion

		#region Public Members

		public static BackgroundTaskRegistrationGroup GetGroup()
		{
			if (Group == null)
				Group = new BackgroundTaskRegistrationGroup(Package.Current.Id.ToString(), "CodeHubApp.BackgroundTasks");
			return Group;
		}
		#endregion
	}
}
