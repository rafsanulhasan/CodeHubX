using CodeHubX.UWP.Models;
using CodeHubX.UWP.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using static CodeHubX.Helpers.CollectionHelper;
using StringHelper = CodeHubX.Helpers.StringHelper;

namespace CodeHubX.UWP.Helpers
{
	internal static class BackgroundTaskBuilderHelper
	{
		private static void Unregister(string name, bool all = true, bool cancelTask = true)
		{
			foreach (var task in BackgroundTaskRegistrationService.GetGroup().AllTasks)
			{
				if (task.Value.Name.ToLower() == name.ToLower())
				{
					task.Value.Unregister(cancelTask);
					if (!all)
					{
						break;
					}
				}
			}
		}

		public static BackgroundTaskBuilder BuildTask(BackgroundTaskBuilderModel model, bool isNetworkRequested = false, bool cancelOnConditionLoss = true)
		{
			// Specify the background task
			var builder = new BackgroundTaskBuilder()
			{
				Name = model.Name
			};
			if (model.Trigger != null)
			{
				builder.SetTrigger(model.Trigger);
			}

			builder.IsNetworkRequested = isNetworkRequested;
			builder.CancelOnConditionLoss = cancelOnConditionLoss;
			if (model.EntryPointType != null)
			{
				builder.TaskEntryPoint = model.EntryPointType.FullName;
			}
			if (!StringHelper.IsNullOrEmptyOrWhiteSpace(model.Name))
			{
				builder.TaskGroup = new BackgroundTaskRegistrationGroup(Package.Current.Id.ToString(), model.Name);
			}

			var conditions = model.GetConditions();
			if (conditions != null && conditions.Count > 0)
			{
				conditions.ForEach(condition => builder.AddCondition(condition));
			}

			return builder;
		}

		public static IBackgroundTaskRegistration Register(this BackgroundTaskBuilder builder, bool unregister = true, bool all = true, bool cancelTask = true)
		{
			if (unregister)
			{
				builder.Unregister(all, cancelTask);
			}

			return builder.Register();
		}

		public static void Unregister(this BackgroundTaskBuilder taskBuilder, bool all = true, bool cancelTask = true) => Unregister(taskBuilder.Name, all, cancelTask);
	}
}
