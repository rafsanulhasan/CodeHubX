using Windows.UI.Xaml.Media;

namespace CodeHubX.UWP.Models
{
	public class SyntaxHighlightStyle
		: CodeHubX.Models.SyntaxHighlightStyle
	{
		public SolidColorBrush ColorOne { get; set; }
		public SolidColorBrush ColorTwo { get; set; }
		public SolidColorBrush ColorThree { get; set; }
		public SolidColorBrush ColorFour { get; set; }
		public SolidColorBrush BackgroundColor { get; set; }
	}
}
