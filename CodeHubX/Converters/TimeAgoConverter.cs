using CodeHubX.Helpers;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	internal class TimeAgoConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			var dt = DateTime.Parse(value.ToString()).ToLocalTime();
			
			return GlobalHelper.ConvertDateToTimeAgoFormat(dt);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
