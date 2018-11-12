using Octokit;
using System;
using System.Collections.Generic;
using CodeHubX.Strings;

namespace CodeHubX.Helpers
{
	public class GlobalHelper
	{
		/*
		  *  Message types are used for ViewModel to ViewModel communication
		  */
		#region Message Types

		public abstract class MessageTypeBase { }

		public partial class AdsEnabledMessageType : MessageTypeBase { }

		public partial class SignOutMessageType : MessageTypeBase { }

		public partial class HasInternetMessageType : MessageTypeBase { }

		public partial class HostWindowBlurMessageType : MessageTypeBase { }

		public partial class ShowWhatsNewPopupMessageType : MessageTypeBase { }

		public class LocalNotificationMessageType
		    : MessageTypeBase
		{
			public string Message { get; set; }
			public string Glyph { get; set; }
		}

		public partial class NoInternet
			: MessageTypeBase
		{
			public LocalNotificationMessageType SendMessage() 
				=>new LocalNotificationMessageType()
				{
					Message = LangResource.notification_NoInternetConnection,
					Glyph = "\uE704",
				};
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

		#region static methods
		/// <summary>
		/// Determines if internet connection is available to device
		/// </summary>
		/// <returns></returns>
		public static Func<bool> IsInternet { get; set; }

		/// <summary>
		/// Converts a Hex string to corresponding SolidColorBrush
		/// </summary>
		/// <param name="hex">rrggbbaa</param>
		/// <returns></returns>
		public static Func<string, object> GetSolidColorBrush { get; set; }

		public static Func<string, object> GetGeomtery { get; set; }

		/// <summary>
		/// Coverts a DateTime to 'time ago' format
		/// </summary>
		/// <returns></returns>
		//public static Func<DateTime, string> ConvertDateToTimeAgoFormat { get; set; }


		/// <summary>
		/// Coverts a DateTime to 'time ago' format
		/// </summary>
		/// <returns></returns>
		public static string ConvertDateToTimeAgoFormat(DateTime dt)
		{
			var ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
			var delta = Math.Abs(ts.TotalSeconds);
			
			if (delta < 60)
			{
				if (ts.Seconds == 1)
					return LangResource.aSecondAgo;
				else
					return $"{ts.Seconds} {LangResource.secondsAgo}";
			}
			if (delta < 120)
				return LangResource.aMinuteAgo;
			if (delta < 2700) // 45 * 60
				return $"{ts.Minutes} {LangResource.aMinuteAgo}";
			if (delta < 5400) // 90 * 60
				return LangResource.anHourAgo;
			if (delta < 86400) // 24 * 60 * 60
				return $"{ts.Hours} {LangResource.hoursAgo}";
			if (delta < 172800) // 48 * 60 * 60
				return LangResource.aDayAgo;
			if (delta < 2592000) // 30 * 24 * 60 * 60
				return $"{ts.Days} {LangResource.daysAgo}";
			if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
			{
				var months = Convert.ToInt32(Math.Floor((double) ts.Days / 30));

				if (months <= 1)
					return LangResource.aMonthAgo;
				else
					return $"{months} {LangResource.monthsAgo}";
			}

			var years = Convert.ToInt32(Math.Floor((double) ts.Days / 365));

			if (years <= 1)
				return LangResource.aYearAgo;
			else
				return $"{years} {LangResource.yearsAgo}";
		}
		#endregion
	}
}