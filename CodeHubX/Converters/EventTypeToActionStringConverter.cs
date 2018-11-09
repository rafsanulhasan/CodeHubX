using CodeHubX.Strings;
using Octokit;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	public class EventTypeToActionStringConverter : IValueConverter
	{
		/// <summary>
		/// Converts Event type to Action string. 
		/// Action string indicates in a verbose manner, what action was done by the actor in an event
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				if (value is Activity activity)
				{
					switch (activity.Type)
					{
						case "IssueCommentEvent":
							return string.Format(LangResource.activity_CommentedIssue, ((IssueCommentPayload) activity.Payload).Issue.Number);

						case "PullRequestReviewCommentEvent":
							return string.Format(LangResource.activity_CommentedPR, ((PullRequestCommentPayload) activity.Payload).PullRequest.Number);

						case "PullRequestEvent":
						case "PullRequestReviewEvent":
							return string.Format(LangResource.activity_ActivityWithPR, ActionConverter(((PullRequestEventPayload) activity.Payload).Action), ((PullRequestEventPayload) activity.Payload).PullRequest.Number);

						case "CommitCommentEvent":
							return LangResource.activity_CommentedCommit;

						case "PushEvent":
							return string.Format(LangResource.activity_PushedCommits, ((PushEventPayload) activity.Payload).Commits.Count);

						case "IssuesEvent":
							return string.Format(LangResource.activity_ActivityWithIssues, ActionConverter(((IssueEventPayload) activity.Payload).Action), ((IssueEventPayload) activity.Payload).Issue.Number);

						case "CreateEvent":
							return LangResource.activity_CreatedBranch;

						case "DeleteEvent":
							return LangResource.activity_DeletedBranch;

						case "ForkEvent":
							return LangResource.activity_ForkedRepository;

						case "WatchEvent":
							return LangResource.activity_StarredRepository;

						case "PublicEvent":
							return LangResource.activity_PublishedRepository;

						case "ReleaseEvent":
							return LangResource.activity_PublishedRelease;

						default:
							return LangResource.activity_DefaultAction;
					}
				}
				else
					return LangResource.activity_DefaultAction;
			}
			catch { return LangResource.activity_DefaultAction; }
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();

		/// <summary>
		/// Convert "opened", "reopened", "closed" word to a localized phrase
		/// </summary>
		/// <param name="action">opened OR reopened OR closed</param>
		/// <returns>Localized phrase</returns>
		public string ActionConverter(string action)
		{
			switch (action)
			{
				case "opened":
					return LangResource.activity_ActionOpened;

				case "reopened":
					return LangResource.activity_ActionReOpened;

				case "closed":
					return LangResource.activity_ActionClosed;

				default:
					return action;
			}
		}
	}
}
