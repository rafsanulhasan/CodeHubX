using CodeHubX.Helpers;
using Octokit;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	internal class IssueDetailStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Issue issue && issue.State.TryParse(out var eventState))
			{
				switch (eventState)
				{
					case ItemState.Open:
						return $"#{issue.Number} opened by {issue.User.Login} {GlobalHelper.ConvertDateToTimeAgoFormat(DateTime.Parse(issue.CreatedAt.ToString()))}";

					case ItemState.Closed:
						return $"#{issue.Number} by {issue.User.Login} was closed {GlobalHelper.ConvertDateToTimeAgoFormat(DateTime.Parse(issue.CreatedAt.ToString()))}";
				}
			}
			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
