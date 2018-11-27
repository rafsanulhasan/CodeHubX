using CodeHubX.Controls;

namespace CodeHubX.Views
{
	public partial class Home : CustomTabbedPage
	{
		public Home()
		{
			InitializeComponent();
			Title = CurrentPage.Title;
		}
	}
}
