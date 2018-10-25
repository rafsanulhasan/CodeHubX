using CodeHubX.UWP.Helpers;
using Octokit;
using System;
using Windows.UI.Xaml.Data;

namespace CodeHubX.UWP.Converters
{
	public class IssueStateToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var issue = (Issue) value;
			if (issue != null)
			{
				if (issue.State.TryParse(out var state))
				{
					switch (state)
					{
						case ItemState.Open:
							return GlobalHelper.GetSolidColorBrush("2CBE4EFF");
					}
				}
			}
			return GlobalHelper.GetSolidColorBrush("CB2431FF");
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotImplementedException();
	}
}
