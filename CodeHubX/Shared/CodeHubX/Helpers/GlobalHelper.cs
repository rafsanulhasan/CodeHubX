using Octokit;
using System;
using System.Collections.Generic;

namespace CodeHubX.Helpers
{
	public class GlobalHelper
	{
		/*
		  *  Message types are used for Viewodel to ViewModel communication
		  */
		#region Message Types

		public abstract class MessageTypeBase { }

		public class AdsEnabledMessageType : MessageTypeBase { }

		public class SignOutMessageType : MessageTypeBase { }

		public class HasInternetMessageType : MessageTypeBase { }

		public class HostWindowBlurMessageType : MessageTypeBase { }

		public class ShowWhatsNewPopupMessageType : MessageTypeBase { }

		public class LocalNotificationMessageType
		    : MessageTypeBase
		{
			public string Message { get; set; }
			public string Glyph { get; set; }
		}

		public class OnOffMessage
			: MessageTypeBase
		{
			public bool InOn { get; set; }
		}

		public class SetHeaderTextMessageType
		    : MessageTypeBase
		{
			public string PageName { get; set; }
		}

		public abstract class UpdateNotificationsCountMessageType
		    : MessageTypeBase
		{
			public int Count { get; set; } = 0;
		}
		public class UpdateAllNotificationsCountMessageType
		    : UpdateNotificationsCountMessageType
		{
		}
		public class UpdateParticipatingNotificationsCountMessageType
		    : UpdateNotificationsCountMessageType
		{
		}
		public class UpdateUnreadNotificationsCountMessageType
		    : UpdateNotificationsCountMessageType
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Client for GitHub client
		/// </summary>
		public static GitHubClient GithubClient { get; set; }

		/// <summary>
		/// Indicates if Ads are visible
		/// </summary>
		public static bool HasAlreadyDonated { get; set; }

		/// <summary>
		/// Maintains a stack of page titles
		/// </summary>
		public static Stack<string> NavigationStack { get; set; } = new Stack<string>();

		/// <summary>
		/// User name of the Authenticated user 
		/// </summary>
		public static string UserLogin { get; set; }

		/// <summary>
		/// Indicates whether user has performed a new Star/Unstar action. Used to update starred repositories
		/// </summary>
		public static bool NewStarActivity { get; set; }

		/// <summary>
		/// List of names and owner names of Trending repositories
		/// </summary>
		public static List<(string, string)> TrendingTodayRepoNames { get; set; }

		/// <summary>
		/// List of names and owner names of Trending repositories
		/// </summary>
		public static List<(string, string)> TrendingWeekRepoNames { get; set; }

		/// <summary>
		/// List of names and owner names of Trending repositories
		/// </summary>
		public static List<(string, string)> TrendingMonthRepoNames { get; set; }

		#endregion
	}

}