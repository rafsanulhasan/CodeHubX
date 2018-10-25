using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CodeHubX.UWP.Converters
{
	internal class ReverseBooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
			=> (bool) value ? Visibility.Collapsed : Visibility.Visible;

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotImplementedException();
	}
}
