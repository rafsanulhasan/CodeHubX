namespace CodeHubX.Helpers
{
	using Models;
	using Prism.Ioc;
	using Prism.Services;
	using Xamarin.Forms;

	public static class AppCenterSettingsHelper
	{
		public static string ToAndroidString(this IAppCenterSettings appCenterSettings)
			=> $"uwp={appCenterSettings.android}";
		public static string ToiOSString(this IAppCenterSettings appCenterSettings)
			=> $"ios={{{appCenterSettings.macOS}";
		public static string ToUWPString(this IAppCenterSettings appCenterSettings)
			=> $"uwp={{{appCenterSettings.uwp}}}";

		public static string ToString(this IAppCenterSettings appCenterSettings)
		{
			var result = $"uwp={appCenterSettings.uwp}";

			if (appCenterSettings.android is string androidKey)
				result += $";android={androidKey}";

			if (appCenterSettings.macOS is string iosKey)
				result += $";ios={iosKey}";

			return result;
		}

		public static string ToString(this IAppCenterSettings appCenterSettings, IContainerProvider container)
		{
			var deviceService = container.Resolve<IDeviceService>();
			return appCenterSettings.ToString(deviceService);
		}

		public static string ToString(this IAppCenterSettings appCenterSettings, IDeviceService deviceService)
		{
			var result = $"uwp={appCenterSettings.uwp}";

			if (appCenterSettings.android is string androidKey)
				result += $";android={androidKey}";
			else if (appCenterSettings.android is IAppCenterAndroidSettings androidSettings)
			{
				if (deviceService.DeviceRuntimePlatform == Device.Android)
				{
					result += ";android=";
					switch (deviceService.Idiom)
					{
						case TargetIdiom.Watch:
							result += $"{androidSettings.watchOS}";
							break;
						case TargetIdiom.Phone:
						case TargetIdiom.Tablet:
						default:
							result += $"{androidSettings.mobile}";
							break;
					}
				}
			}

			if (appCenterSettings.macOS is string iosKey)
				result += $";ios={iosKey}";
			else if (appCenterSettings.android is IAppCenterMacOSSettings macOSSettings)
			{
				if (deviceService.DeviceRuntimePlatform == Device.iOS)
				{
					result += ";ios=";
					switch (deviceService.Idiom)
					{
						case TargetIdiom.Watch:
							result += $"{macOSSettings.watchOS}";
							break;
						case TargetIdiom.Phone:
						case TargetIdiom.Tablet:
						default:
							result += $"{macOSSettings.iOS}";
							break;
					}
				}
			}

			return result;
		}
	}
}
