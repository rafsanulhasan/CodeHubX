using CodeHubX.UWP.Helpers;
using Octokit;
using System;
using Windows.UI.Xaml.Data;

namespace CodeHubX.UWP.Converters
{
	internal class IssueDetailStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var issue = (Issue) value;

			if (issue.State.TryParse(out var eventState))
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

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotImplementedException();
	}
}
