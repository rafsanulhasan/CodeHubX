using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public sealed partial class WhatsNewPopupControl : ContentView
	{
		public WhatsNewPopupControl()
		{
			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += CloseWhatsNew_Tapped;
			//CloseWhatsNew.GentureRecognizers.Add(tapGestureRecognizer);
		}

		public string WhatsNewText
		{
			get => (string) GetValue(WhatsNewTextroperty);
			set => SetValue(WhatsNewTextroperty, value);
		}

		public static readonly BindableProperty WhatsNewTextroperty
			= BindableProperty.Create(nameof(WhatsNewText), typeof(string), typeof(WhatsNewPopupControl));

		private void CloseWhatsNew_Tapped(object sender, EventArgs e) =>
			//await this.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
			IsVisible = false;
	}
}
