using Octokit;
using System;
using System.Collections.Generic;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace CodeHubX.UWP.Helpers
{
	public class GlobalHelper
		: CodeHubX.Helpers.GlobalHelper
	{
		/*
		  *  Message types are used for ViewModel to ViewModel communication
		  */
		#region Message Types

		public class NoInternet
			: MessageTypeBase
		{
			public LocalNotificationMessageType SendMessage()
				=> new LocalNotificationMessageType()
				{
					Message = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("notification_NoInternetConnection"),
					Glyph = "\uE704",
				};
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
		public static bool IsInternet()
		{
			var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
			return connectionProfile != null &&
							   connectionProfile.GetNetworkConnectivityLevel() ==
							   NetworkConnectivityLevel.InternetAccess;

		}

		/// <summary>
		/// Converts a Hex string to corressponding SolidColorBrush
		/// </summary>
		/// <param name="hex">rrggbbaa</param>
		/// <returns></returns>
		public static SolidColorBrush GetSolidColorBrush(string hex)
		{
			var r = (byte) Convert.ToUInt32(hex.Substring(0, 2), 16);
			var g = (byte) Convert.ToUInt32(hex.Substring(2, 2), 16);
			var b = (byte) Convert.ToUInt32(hex.Substring(4, 2), 16);
			var a = (byte) Convert.ToUInt32(hex.Substring(6, 2), 16);
			var myBrush = new SolidColorBrush(Color.FromArgb(a, r, g, b));
			return myBrush;
		}

		public static Geometry GetGeomtery(string path)
		{
			var sym = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" + path + "</Geometry>";
			return (Geometry) XamlReader.Load(sym);
		}

		/// <summary>
		/// Coverts a DateTime to 'time ago' format
		/// </summary>
		/// <returns></returns>
		public static string ConvertDateToTimeAgoFormat(DateTime dt)
		{
			var ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
			var delta = Math.Abs(ts.TotalSeconds);

			var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

			if (delta < 60)
			{
				if (ts.Seconds == 1)
					return languageLoader.GetString("aSecondAgo");
				else
					return $"{ts.Seconds} {languageLoader.GetString("secondsAgo")}";
			}
			if (delta < 120)
				return languageLoader.GetString("aMinuteAgo");
			if (delta < 2700) // 45 * 60
				return $"{ts.Minutes} {languageLoader.GetString("minutesAgo")}";
			if (delta < 5400) // 90 * 60
				return languageLoader.GetString("anHourAgo");
			if (delta < 86400) // 24 * 60 * 60
				return $"{ts.Hours} {languageLoader.GetString("hoursAgo")}";
			if (delta < 172800) // 48 * 60 * 60
				return languageLoader.GetString("aDayAgo");
			if (delta < 2592000) // 30 * 24 * 60 * 60
				return $"{ts.Days} {languageLoader.GetString("daysAgo")}";
			if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
			{
				var months = Convert.ToInt32(Math.Floor((double) ts.Days / 30));

				if (months <= 1)
					return languageLoader.GetString("oneMonthAgo");
				else
					return $"{months} {languageLoader.GetString("monthsAgo")}";
			}

			var years = Convert.ToInt32(Math.Floor((double) ts.Days / 365));

			if (years <= 1)
				return languageLoader.GetString("oneYearAgo");
			else
				return $"{years} {languageLoader.GetString("yearsAgo")}";
		}
		#endregion
	}

}