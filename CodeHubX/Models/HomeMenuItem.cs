namespace CodeHubX.Models
{
	public enum MenuItemType
	{
		Browse,
		About,
		Feeds,
		NoNetwork
	}
	public class HomeMenuItem
	{
		public MenuItemType Id { get; set; }

		public string Title { get; set; }
	}
}
