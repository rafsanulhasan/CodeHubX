using CodeHubX.Helpers;
using Octokit;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	internal class PullRequestDetailStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is PullRequest pr && pr.State.TryParse(out var eventState))
			{
				switch (eventState)
				{
					case ItemState.Open:
						return $"#{pr.Number} opened by {pr.User.Login} {GlobalHelper.ConvertDateToTimeAgoFormat(DateTime.Parse(pr.CreatedAt.ToString()))}";

					case ItemState.Closed:
						return $"#{pr.Number} by {pr.User.Login} was merged {GlobalHelper.ConvertDateToTimeAgoFormat(DateTime.Parse(pr.CreatedAt.ToString()))}";
				}
			}

			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
