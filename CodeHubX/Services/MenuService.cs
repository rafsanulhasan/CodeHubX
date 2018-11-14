using CodeHubX.Helpers;
using CodeHubX.Models;
using CodeHubX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CodeHubX.Services
{
	public static class MenuService
	{
		private static readonly IDictionary<int, NavigationPage> _MenuPages = _MenuPages ??
			new Dictionary<int, NavigationPage>();

		public static IReadOnlyDictionary<int, NavigationPage> MenuPages
			=> _MenuPages.ToReadOnlyDictionary();

		public static void Add(KeyValuePair<int, NavigationPage> pageItem)
		{
			if (!_MenuPages.ContainsKey(pageItem.Key))
				_MenuPages.Add(pageItem);
		}

		public static void Add(Tuple<int, NavigationPage> pageItem)
			=> Add(new KeyValuePair<int, NavigationPage>(pageItem.Item1, pageItem.Item2));

		public static void Add((int Id, NavigationPage Page) pageItem)
			=> Add(new KeyValuePair<int, NavigationPage>(pageItem.Id, pageItem.Page));

		public static void Add(int id, NavigationPage page)
			=> Add((id, page));

		public static void Clear()
			=> _MenuPages.Clear();

		public static bool ContainsKey(int id)
			=> _MenuPages.ContainsKey(id);

		public static bool ContainsPage(NavigationPage page)
			=> _MenuPages?.Values?.Single(t => t == page) != null;

		public static bool ContainsValue(NavigationPage page)
			=> _MenuPages?.Values?.Contains(page) ?? false;

		public static NavigationPage Get(int id)
		{
			switch (id)
			{
				case (int) MenuItemType.About:
					return new NavigationPage(new AboutPage());
				case (int) MenuItemType.Feeds:
					return new NavigationPage(new FeedsPage());
				default:
					return new NavigationPage(new ItemsPage());
			}
		}

		public static int Get<TPage>(TPage page)
			where TPage : Page
		{
			if (typeof(TPage) == typeof(AboutPage))
				return (int) MenuItemType.About;
			if (typeof(TPage) == typeof(FeedsPage))
				return (int) MenuItemType.About;
			return (int) MenuItemType.Browse;
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
