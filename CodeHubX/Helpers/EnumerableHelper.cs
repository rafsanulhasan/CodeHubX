using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CodeHubX.Helpers
{
	public static class EnumerableHelper
	{
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
