using CodeHubX.UWP.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace CodeHubX.UWP.Converters
{
	internal class ColorStringToColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
			=> GlobalHelper.GetSolidColorBrush((value as string) + "FF");

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotImplementedException();
	}
}
