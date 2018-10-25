﻿using Octokit;
using System;
using Windows.UI.Xaml.Data;

namespace CodeHubX.UWP.Converters
{
	internal class EventTypeToCommentStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
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

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotImplementedException();
	}
}
