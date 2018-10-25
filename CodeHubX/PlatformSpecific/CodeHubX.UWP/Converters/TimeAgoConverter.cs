﻿using CodeHubX.UWP.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace CodeHubX.UWP.Converters
{
	public class TimeAgoConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
				return null;

			var dt = DateTime.Parse(value.ToString());
			return GlobalHelper.ConvertDateToTimeAgoFormat(dt);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotSupportedException();
	}
}
