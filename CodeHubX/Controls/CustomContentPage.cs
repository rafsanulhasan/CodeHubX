using CodeHubX.ViewModels;
using Prism.Navigation;
using Xamarin.Forms;

namespace CodeHubX.Controls
{
	public class CustomContentPage
		: ContentPage
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
	}
}
