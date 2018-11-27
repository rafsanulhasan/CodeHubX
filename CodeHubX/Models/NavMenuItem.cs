namespace CodeHubX.Models
{
	public enum NavMenuItemType
	{
		Browse,
		About,
		Feeds,
		NoNetwork
	}

	public enum NavMenuType
	{
		System = 0,
		Navigation = 1
	}

	public enum BadgeType
	{
		Information = 0,
		Alert = 1
	}

	public class NavBadge<T>
	{
		public BadgeType Type { get; private set; }
		public T Value { get; private set; }

		public NavBadge(BadgeType type, T value)
		{
			Type = type;
			Value = value;
		}
	}

	public class NavBadge
		: NavBadge<string>
	{
		public NavBadge(BadgeType type, string value)
			: base(type, value)
		{

		}
	}

	public class NavMenuItem
	{
		public NavBadge Badge { get; set; }

		public NavMenuItemType Id { get; set; }

		public NavMenuType MenuType { get; set; }

		public string MenuTitle { get; set; }

		public string MenuTitleResourceName { get; set; }

		public string Url { get; set; }

		public string PageKey { get; set; }
	}
}
