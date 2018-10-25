using CodeHub.Helpers;
using Octokit;
using System;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
	internal class PullRequestDetailStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var pr = (PullRequest) value;

			if (pr.State.TryParse(out var eventState))
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

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotImplementedException();
	}
}
