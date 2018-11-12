using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	internal class ForegroundFromBackgroundConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//var background = GlobalHelper.GetSolidColorBrush((value as string) + "FF");
			//return new SolidColorBrush(PerceivedBrightness(background) > 130 ? Colors.Black : Colors.White);
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();

		private int PerceivedBrightness(Color c)
			=> (int) Math.Sqrt(
					c.R * c.R * .299 +
					c.G * c.G * .587 +
					c.B * c.B * .114);
	}
}