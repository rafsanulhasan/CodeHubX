using CodeHubX.Services;
using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodeHubX.Helpers
{
	public class NotificationModel
	{
		public long RepositoryId { get; private set; }

		public int Number { get; private set; }

		public Issue Issue { get; private set; }

		public PullRequest PullRequest { get; private set; }

		public string Subtitle { get; private set; }

		private NotificationModel(long repositoryId, string subTitle)
		{
			RepositoryId = repositoryId;
			Subtitle = subTitle;
		}

		private void SetNotificationType<T>(T item)
		{
			if (item is Issue issue)
			{
				Issue = issue;
				Number = issue.Number;
			}
			else if (item is PullRequest pr)
			{
				PullRequest = pr;
				Number = pr.Number;
			}
		}

		public NotificationModel(
		    long repositoryId,
		    Issue item,
		    string subtitle
		)
		    : this(repositoryId, subtitle)
			=> SetNotificationType(item);

		public NotificationModel(
		    long repositoryId,
		    PullRequest item,
		    string subtitle
		)
		    : this(repositoryId, subtitle)
			=> SetNotificationType(item);

		public bool IsIssue()
			=> Issue != null && PullRequest == null;

		public bool IsPullRequest()
			=> Issue == null && PullRequest != null;

		public void SetSubtitle(string subtitle)
			=> Subtitle = subtitle;
	}

	public static class OctokitNotificationHelper
	{
		public static async Task<NotificationModel> ProcessNotification(this Octokit.Notification notification)
		{
			NotificationModel result = null;
			var isIssue = notification.Subject.Type.ToLower() == "issue";
			var isPR = notification.Subject.Type.ToLower() == "pullrequest";
			if (int.TryParse(notification.Subject.Url.Split('/').Last().Split('#').First(), out var number))
			{
				var subtitle = "";
				var repo = notification.Repository;
				var repoName = repo.FullName ?? repo.Name;
				if (isIssue)
				{
					var issue = await IssueUtility.GetIssue(repo.Id, number);
					subtitle = $"Issue {number}";
					result = new NotificationModel(repo.Id, issue, subtitle);
				}
				else if (isPR)
				{
					var pr = await PullRequestUtility.GetPullRequest(repo.Id, number);
					subtitle = $"PR {number}";
					result = new NotificationModel(repo.Id, pr, subtitle);
				}
				subtitle = !StringHelper.IsNullOrEmptyOrWhiteSpace(subtitle)
						? $"{subtitle} in {repoName}"
						: repoName;
				result.SetSubtitle(subtitle);
			}

			return result ?? throw new NullReferenceException(nameof(result));
		}
	}
}
