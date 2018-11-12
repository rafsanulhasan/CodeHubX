using CodeHubX.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CodeHubX.Services
{
	public static class MenuService
	{
		private static readonly IDictionary<int, NavigationPage> _MenuPages =
			new Dictionary<int, NavigationPage>();

		public static IReadOnlyDictionary<int, NavigationPage> MenuPages
			=> _MenuPages.ToReadOnlyDictionary();

		public static void Add(KeyValuePair<int, NavigationPage> pageItem)
			=> _MenuPages.Add(pageItem);

		public static void Add(Tuple<int, NavigationPage> pageItem)
			=> Add(new KeyValuePair<int, NavigationPage>(pageItem.Item1, pageItem.Item2));

		public static void Add((int Id, NavigationPage Page) pageItem)
			=> Add(pageItem);

		public static void Add(int id, NavigationPage page)
			=> Add((id, page));

		public static void Clear()
			=> _MenuPages.Clear();

		public static bool ContainsKey(int id)
			=> _MenuPages.ContainsKey(id);

		public static bool ContainsPage(NavigationPage page)
			=> _MenuPages.Values.Single(t => t == page && ReferenceEquals(t, page)) != null;

		public static bool ContainsValue(NavigationPage page)
			=> _MenuPages.Values.Contains(page);

		public static (int Id, NavigationPage Page) Get(int id)
		{
			var pageItem = _MenuPages.Single(mp => mp.Key == id);
			return (pageItem.Key, pageItem.Value);
		}

		public static (int Id, NavigationPage Page) Get(Page page)
		{
			var pageItem = _MenuPages.Single(mp => mp.Value == page);
			return (pageItem.Key, pageItem.Value);
		}

		public static void Remove(KeyValuePair<int, NavigationPage> pageItem)
			=> _MenuPages.Remove(pageItem);

		public static void Remove(Tuple<int, NavigationPage> pageItem)
			=> _MenuPages.Remove(new KeyValuePair<int, NavigationPage>(pageItem.Item1, pageItem.Item2));

		public static void Remove((int id, NavigationPage page) pageItem)
			=> _MenuPages.Remove(new KeyValuePair<int, NavigationPage>(pageItem.id, pageItem.page));

		public static void Remove(int key, NavigationPage page = null)
		{
			if (page is null)
				_MenuPages.Remove(key);
			else
				Remove((key, page));
		}
	}
}
