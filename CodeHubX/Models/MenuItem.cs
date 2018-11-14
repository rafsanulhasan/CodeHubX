namespace CodeHubX.Models
{
	public enum MenuItemType
	{
		Browse,
		About,
		Feeds,
		NoNetwork
	}

	public enum MenuType
	{
		System = 0,
		Navigation = 1
	}

	public enum BadgeType
	{
		Information = 0,
		Alert = 1
	}

	public class Badge<T>
	{
		public BadgeType Type { get; private set; }
		public T Value { get; private set; }

		public Badge(BadgeType type, T value)
		{
			Type = type;
			Value = value;
		}
	}

	public class Badge
		: Badge<string>
	{
		public Badge(BadgeType type, string value)
			: base(type, value)
		{

		}
	}

	public class MenuItem
	{
		public Badge Badge { get; set; }

		public MenuItemType Id { get; set; }

		public MenuType MenuType { get; set; }

		public string MenuTitle { get; set; }

		public string MenuTitleResourceName { get; set; }

		public int Number
		{
			get => (int) Id;
			set
			{
				switch (value)
				{
					case (int) MenuItemType.About:
						Id = MenuItemType.About;
						break;
					default:
					case (int) MenuItemType.Feeds:
						Id = MenuItemType.Feeds;
						break;
					case (int) MenuItemType.NoNetwork:
						Id = MenuItemType.NoNetwork;
						break;
				}
			}
		}

		public string PageTitle { get; set; }

		public string PageTitleResourceName { get; set; }
	}
}
