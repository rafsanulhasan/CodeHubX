using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	internal class NotifTypeToSymbolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((string) value)
			{
				case "Issue":
					return "";

				case "PullRequest":
					return "";

				default:
					return "";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
