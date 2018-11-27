using CodeHubX.ViewModels;
using Xamarin.Forms;

namespace CodeHubX.Controls
{
	public class CustomTabbedPage
		: TabbedPage
	{
		protected override bool OnBackButtonPressed()
		{
			if (BindingContext is ViewModelBase vm)
			{
				vm.GoBack();
				return true;
			}
			return false;
		}

		protected override void OnCurrentPageChanged()
		{
			base.OnCurrentPageChanged();
			Title = CurrentPage?.Title ?? "";
		}
	}
}
