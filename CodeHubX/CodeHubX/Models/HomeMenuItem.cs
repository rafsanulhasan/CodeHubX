namespace CodeHubX.Models
{
	public enum MenuItemType
	{
		Browse,
		About,
		Feeds,
	}
	public class HomeMenuItem
	{
		public MenuItemType Id { get; set; }

		public string Title { get; set; }
	}
}
