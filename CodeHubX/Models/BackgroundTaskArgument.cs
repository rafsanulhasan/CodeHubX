using System;
using System.Threading.Tasks;

namespace CodeHubX.Models
{
	public class BackgroundTaskArgument
	{
		public Func<string, Task> MarkNotificationAsRead { get; set; }

		public string Action { get; set; }

		public string Filter { get; set; }

		public bool IsGhost { get; set; }

		public string State { get; set; }

		public string What { get; set; }

		public string Where { get; set; }

		public bool WillSendMessage { get; set; }

		public bool WillUpdateBadge { get; set; }

		public string NotificationId { get; set; }
	}
}
