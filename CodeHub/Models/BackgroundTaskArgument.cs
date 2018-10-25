using System;
using System.Threading.Tasks;
using static CodeHub.Helpers.QueryStringHelper;
using static CodeHub.Helpers.ValueSetHelper;
using QueryString = Microsoft.QueryStringDotNET.QueryString;
using ValueSet = Windows.Foundation.Collections.ValueSet;

namespace CodeHub.Models
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

		public static BackgroundTaskArgument Parse(QueryString query) 
			=> Validate(query).ToBackgroudTaskArgument();

		public static BackgroundTaskArgument Parse(ValueSet valueSet) 
			=> Validate(valueSet).ToBackgroudTaskArgument();
	}
}
