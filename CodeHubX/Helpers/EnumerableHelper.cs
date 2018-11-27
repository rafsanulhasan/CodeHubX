using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CodeHubX.Helpers
{
	public static class EnumerableHelper
	{
		public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> enumerable, int count)
		{
			ICollection<T> result = new List<T>();
			var j = 1;
			for (var i = enumerable.Count() - 1; i > 0; i--)
			{
				if (j > count || j < count)
					break;
				var currentItem = enumerable.ElementAt(i);
				result.Add(currentItem);
				j++;
			}
			return result;
		}

		public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
			=> new ReadOnlyCollection<T>(enumerable.ToList());

		public static ReadOnlyObservableCollection<T> ToReadOnlyObservableCollection<T>(this IEnumerable<T> collection)
			=> collection
				.ToList()
				.ToReadOnlyObservableCollection();

		public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> enumerable)
			=> new ReadOnlyCollection<T>(enumerable.ToList());
	}
}
