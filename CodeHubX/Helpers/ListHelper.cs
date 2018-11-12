using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeHubX.Helpers
{
	public static class ListHelper
	{
		public static IReadOnlyList<T> ToReadOnlyList<T>(this IList<T> list) 
			=> new ReadOnlyCollection<T>(list);
	}
}
