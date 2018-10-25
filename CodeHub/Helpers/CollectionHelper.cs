using CodeHub.Models;
using CodeHub.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Background;

namespace CodeHub.Helpers
{
	internal class UniqueCollection
		 : Collection<object>
	{

		public new void Add(object item)
		{
			if (Items.Contains(item))
			{
				throw new InvalidOperationException($"{item} already exists");
			}

			base.Add(item);
		}
		protected override void InsertItem(int index, object item)
		{
			if (Items.Contains(item))
			{
				throw new InvalidOperationException($"{item} already exists");
			}

			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, object item)
		{
			if (Items.Contains(item))
			{
				throw new InvalidOperationException($"{item} already exists");
			}

			base.SetItem(index, item);
		}
	}

	internal class UniqueCollection<T>
	   : Collection<T>
	{

		public new void Add(T item)
		{
			if (Items.Contains(item))
			{
				throw new InvalidOperationException($"{item} already exists");
			}

			base.Add(item);
		}
		protected override void InsertItem(int index, T item)
		{
			if (Items.Contains(item))
			{
				throw new InvalidOperationException($"{item} already exists");
			}

			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, T item)
		{
			if (Items.Contains(item))
			{
				throw new InvalidOperationException($"{item} already exists");
			}

			base.SetItem(index, item);
		}
	}

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

		public static ICollection Combine(this ICollection collection, IEnumerable combinableCollection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			if (combinableCollection == null)
			{
				throw new ArgumentNullException(nameof(combinableCollection));
			}

			var result = new UniqueCollection();


			if (collection.Count > 0)
			{
				collection.ForEach(c => result.Add(c));
			}
			if (combinableCollection.Count() > 0)
			{
				combinableCollection.ForEach(o =>
				{
					if (!result.Contains(o))
					{
						result.Add(o);
					}
				});
			}
			return result;
		}

		public static ICollection<T> Combine<T>(this ICollection<T> collection, IEnumerable<T> combinableCollection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			if (combinableCollection == null)
			{
				throw new ArgumentNullException(nameof(combinableCollection));
			}
			var result = new UniqueCollection<T>();
			if (collection.Count > 0)
			{
				collection.ForEach(c => result.Add(c));
			}
			if (combinableCollection.Count() > 0)
			{
				combinableCollection.ForEach(o =>
				{
					if (!result.Contains(o))
					{
						result.Add(o);
					}
				});
			}
			return result;
		}

		public static bool Contains<T>(this ICollection<T> collection, T item)
		{
			foreach (var c in collection)
			{
				if (item.Equals(c))
				{
					return true;
				}
			}
			return false;
		}
		public static bool Contains<T, TEqualityComparer>(this ICollection<T> collection, T item, TEqualityComparer comparer)
		    where TEqualityComparer : IEqualityComparer<T>
		{
			foreach (var c in collection)
			{
				return comparer?.Equals(item, c) ?? item.Equals(c);
			}
			return false;
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

		public static bool Contains<T>(this IEnumerable<T> collection, T item)
		{
			foreach (var c in collection)
			{
				if (item.Equals(c))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Contains<T, TEqualityComparer>(this IEnumerable<T> collection, T item, TEqualityComparer comparer)
		    where TEqualityComparer : IEqualityComparer<T>
		{
			foreach (var c in collection)
			{
				return comparer?.Equals(item, c) ?? item.Equals(c);
			}
			return false;
		}

		public static void ForEach(this ICollection collection, Action<object> predicate)
		{
			foreach (var item in collection)
			{
				predicate(item);
			}
		}

		public static void ForEach<T>(this ICollection<T> enumerable, Action<T> predicate)
		{
			foreach (var item in enumerable)
			{
				predicate(item);
			}
		}

		public static void ForEach(this IEnumerable collection, Action<object> predicate)
		{
			foreach (var item in collection)
			{
				predicate(item);
			}
		}

		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> predicate)
		{
			foreach (var item in enumerable)
			{
				predicate(item);
			}
		}

		public static void ForEach(this IQueryable queryable, Action<object> predicate)
		{
			foreach (var item in queryable)
			{
				predicate(item);
			}
		}

		public static void ForEach<T>(this IQueryable<T> queryable, Action<T> predicate)
		{
			foreach (var item in queryable)
			{
				predicate(item);
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
