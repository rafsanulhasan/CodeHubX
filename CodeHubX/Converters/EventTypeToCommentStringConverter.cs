using Octokit;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	internal class EventTypeToCommentStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var activity = value as Activity;

			switch (activity.Type)
			{
				case "IssueCommentEvent":
					return ((IssueCommentPayload) activity.Payload).Comment.Body;

				case "PullRequestReviewCommentEvent":
					return ((PullRequestCommentPayload) activity.Payload).Comment.Body;

				case "PushEvent":
					return ((PushEventPayload) activity.Payload).Ref;

				case "CommitCommentEvent":
					return ((CommitCommentPayload) activity.Payload).Comment.Body;

				default:
					return string.Empty;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
