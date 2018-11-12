using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public sealed partial class MarkdownControl : TabbedPage
	{
		private void MarkdownText_Changed(object sender, TextChangedEventArgs args)
		{
			
		}

		//public string MarkdownText
		//{
		//	get => (string) GetValue(MarkdownView.MarkdownProperty);
		//	set => SetValue(markdownView.Markdown, value);
		//}

		//public static readonly BindableProperty MarkdownTextProperty =
		//  BindableProperty.Create(nameof(MarkdownText), typeof(string), typeof(MarkdownControl));
	}
}
