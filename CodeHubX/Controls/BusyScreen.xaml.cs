using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public sealed partial class BusyScreen : ContentView
	{
		public static readonly BindableProperty BusyTextProperty = BindableProperty.Create(nameof(BusyText), typeof(string), typeof(BusyScreen));
		public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(BusyScreen));

		public BusyScreen()
		{
		}

		public string BusyText
		{
			get => (string) GetValue(BusyTextProperty);
			set => SetValue(BusyTextProperty, value);
		}

		public bool IsBusy
		{
			get => (bool) GetValue(IsBusyProperty);
			set => SetValue(IsBusyProperty, value);
		}

	}
}