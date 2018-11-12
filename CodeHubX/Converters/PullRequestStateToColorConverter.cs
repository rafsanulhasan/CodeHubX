using CodeHubX.Helpers;
using Octokit;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	public class PullRequestStateToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is PullRequest pr && pr.State.TryParse(out var state))
			{
				switch (state)
				{
					case ItemState.Open:
						return GlobalHelper.GetSolidColorBrush("2CBE4EFF");
				}
			}

			return GlobalHelper.GetSolidColorBrush("CB2431FF");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}