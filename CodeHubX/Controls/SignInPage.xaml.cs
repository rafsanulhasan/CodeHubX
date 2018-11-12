using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public sealed partial class SignInPage : ContentView
	{
		public static readonly BindableProperty CommandProperty
			= BindableProperty.Create("SignInCommand", typeof(ICommand), typeof(SignInPage), null);

		public ICommand SignInCommand
		{
			get => (ICommand) GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}
	}
}
