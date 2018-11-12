using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	public class EventTypeToSymbolConverter : IValueConverter
	{
		/// <summary>
		/// Converts Event type to an SVG symbol
		/// The symbol indicates what action was done in an event
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var symbolPath = string.Empty;
			switch ((string) value)
			{
				case "ForkEvent":
					symbolPath = "repo-forked.svg";
					break;

				case "PullRequestEvent":
					symbolPath = "pr.svg";
					break;

				case "IssueCommentEvent":
				case "PullRequestReviewCommentEvent":
				case "CommitCommentEvent":
					symbolPath = "comment.svg";
					break;

				case "PushEvent":
					symbolPath = "push.svg";
					break;

				case "IssuesEvent":
					symbolPath = "issue.svg";
					break;

				case "WatchEvent":
					symbolPath = "watch.svg";
					break;

				default:
					symbolPath = "feed.svg";
					break;
			}
			symbolPath = Path.Combine("oct-icons", symbolPath);
			switch (Device.RuntimePlatform)
			{
				case Device.Android:
				case Device.iOS:
				default:
					return symbolPath;
				case Device.UWP:
					return Path.Combine("Assets", symbolPath);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}