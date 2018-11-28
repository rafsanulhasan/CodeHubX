using Prism.Ioc;
using Prism.Services;

namespace CodeHubX.Helpers
{
	using Models;

	public static class AppCenterConfigurationHelper
	{
		public static string ToAndroidString(this IAppCenterConfiguration appCenterConfig)
			=> appCenterConfig.settings.ToAndroidString();
		public static string ToiOSString(this IAppCenterConfiguration appCenterConfig)
			=> appCenterConfig.settings.ToiOSString();
		public static string ToUWPString(this IAppCenterConfiguration appCenterConfig)
			=> appCenterConfig.settings.ToUWPString();

		public static string ToString(this IAppCenterConfiguration appCenterConfig)
			=> appCenterConfig.settings.ToString();

		public static string ToString(this IAppCenterConfiguration appCenterConfig, IContainerProvider container)
			=> appCenterConfig.settings.ToString(container);

		public static string ToString(this IAppCenterConfiguration appCenterConfig, IDeviceService deviceService)
			=> appCenterConfig.ToString(deviceService);
	}
}
