using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	internal class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> !(bool) value;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> !(bool) value;
	}
}
