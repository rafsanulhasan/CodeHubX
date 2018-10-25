using CodeHubX.UWP.Models;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;

namespace CodeHubX.UWP.Helpers
{
	internal static class CollectionHelper
	{
		private static void GenerateDefault<T>(this ICollection<BackgroundTaskBuilder> tasks, ref T[] values)
		{
			if (tasks != null && tasks.Count > 0)
			{

				values = new T[tasks.Count];
				var i = 0;
				foreach (var t in tasks)
				{
					if (values[i] is bool boolValue)
					{
						boolValue = false;
					}
					else if (values[i] is BackgroundTaskRegistrationGroup groupValue)
					{
						groupValue = null;
					}
					else
					{
						values[i] = default;
					}
					i++;
				}
			}
		}

		private static void GenerateDefault<T>(this ICollection<BackgroundTaskBuilderModel> tasks, ref T[] values)
		{
			if (tasks != null && tasks.Count > 0)
			{

				values = new T[tasks.Count];
				var i = 0;
				foreach (var t in tasks)
				{
					if (values[i] is bool boolValue)
					{
						boolValue = false;
					}
					else if (values[i] is BackgroundTaskRegistrationGroup groupValue)
					{
						groupValue = null;
					}
					else
					{
						values[i] = default;
					}
					i++;
				}
			}
		}

		private static void Validate<T>(this ICollection<BackgroundTaskBuilder> tasks, ref T[] array)
		{
			if (tasks == null)
			{
				throw new ArgumentNullException(nameof(tasks));
			}

			if (tasks.Count > 0)
			{
				if (array == null)
				{
					tasks.GenerateDefault(ref array);
				}
				if (tasks.Count != array.Length)
				{
					throw new IndexOutOfRangeException($"Length of {nameof(tasks)} and {nameof(array)} must be same");
				}
			}
		}

		private static void Validate<T>(this ICollection<BackgroundTaskBuilderModel> tasks, ref T[] array)
		{
			if (tasks == null)
			{
				throw new ArgumentNullException(nameof(tasks));
			}

			if (tasks.Count > 0)
			{
				if (array == null)
				{
					tasks.GenerateDefault(ref array);
				}
				if (tasks.Count != array.Length)
				{
					throw new IndexOutOfRangeException($"Length of {nameof(tasks)} and {nameof(array)} must be same");
				}
			}
		}

		public static IEnumerable<BackgroundTaskBuilder> BuildTasks(ICollection<BackgroundTaskBuilderModel> backgroundTasks, bool[] isNetworkRequested = null, bool[] cancelOnConditionLoss = null, BackgroundTaskRegistrationGroup[] groups = null)
		{
			if (backgroundTasks == null)
			{
				throw new ArgumentNullException(nameof(backgroundTasks));
			}

			if (backgroundTasks.Count > 0)
			{
				backgroundTasks.Validate(ref isNetworkRequested);
				backgroundTasks.Validate(ref cancelOnConditionLoss);
				backgroundTasks.Validate(ref groups);
				var i = 0;
				foreach (var task in backgroundTasks)
				{
					yield return BackgroundTaskBuilderHelper.BuildTask(task, isNetworkRequested[i], cancelOnConditionLoss[i]);
					i++;
				}
			}
		}

		public static IEnumerable<IBackgroundTaskRegistration> Register(this ICollection<BackgroundTaskBuilder> builders, bool unregister = true, bool all = true, bool cancelTask = true)
		{
			foreach (var builder in builders)
			{
				yield return builder.Register(unregister, all, cancelTask);
			}
		}

		public static IEnumerable<IBackgroundTaskRegistration> Register(this ICollection<BackgroundTaskBuilder> builders, ApplicationTrigger[] triggers, bool unregister = true, bool all = true, bool cancelTask = true)
		{
			builders.Validate(ref triggers);
			var i = 0;
			foreach (var builder in builders)
			{
				builder.SetTrigger(triggers[i]);
				yield return builder.Register(unregister, all, cancelTask);
				i++;
			}
		}
	}
}
