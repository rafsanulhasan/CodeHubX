using GalaSoft.MvvmLight;

namespace CodeHubX.Models
{
	public class SyntaxHighlightStyle : ObservableObject
	{
		public string Name { get; set; }
		public bool IsLineNumbersVisible { get; set; }
	}
}
