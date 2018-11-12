using CodeHubX.Services;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	public class L10nConverter
		 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType == typeof(string))
			{
				if (parameter is string param)
					return L10n.Localize(value as string, param);
				else
					return L10n.Localize(value as string, "");
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
