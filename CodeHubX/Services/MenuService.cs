using CodeHubX.Helpers;
using CodeHubX.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeHubX.Services
{
	public class MenuService
		: IMenuService
	{
		private readonly ICollection<NavMenuItem> _Menus;

		private const string AboutTitle = "About";
		private const string AboutKey = "About";
		private const string HomeTitle = "Home";
		private const string HomeKey = "Home?selectedTab=Feeds";

		public IReadOnlyList<NavMenuItem> Menus
			=> _Menus.ToReadOnlyList();

		public MenuService()
		{
			_Menus = _Menus
				 ?? new List<NavMenuItem>()
				    {
						new NavMenuItem{ MenuTitle = HomeTitle, PageKey = HomeKey },
						new NavMenuItem{ MenuTitle = AboutTitle, PageKey = AboutKey },
				    };
		}

		public void Add(NavMenuItem pageItem)
		{
			if (!_Menus.Contains(pageItem))
				_Menus.Add(pageItem);
		}

		public void Clear()
			=> _Menus.Clear();

		public bool Contains(NavMenuItem menuItem)
			=> _Menus.Contains(menuItem);

		public void Remove(NavMenuItem pageItem)
			=> _Menus.Remove(pageItem);
	}
}
