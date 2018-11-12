using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeHubX.Helpers
{
	public static class DictionaryHelper
	{
		public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dic, Action<KeyValuePair<TKey, TValue>> predicate)
		{
			foreach (var item in dic)
				predicate(item);
		}

		public static IReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dic) 
			=> new ReadOnlyDictionary<TKey, TValue>(dic);
	}
}
