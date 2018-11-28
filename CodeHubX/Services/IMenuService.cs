using CodeHubX.Models;
using System.Collections.Generic;

namespace CodeHubX.Services
{
	public interface IMenuService
	{
		IReadOnlyList<NavMenuItem> Menus { get; }

		void Add(NavMenuItem pageItem);

		void Clear();

		bool Contains(NavMenuItem menuItem);

		void Remove(NavMenuItem pageItem);
	}
}
