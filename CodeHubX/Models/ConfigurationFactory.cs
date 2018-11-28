using CodeHubX.Services;
using Newtonsoft.Json;
using Prism.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeHubX.Models
{
	public static class ConfigurationFactory
	{
		public static async Task<IApplicationConfiguration> GetApplicationConfiguration(IFileStorage fileStorage, IDeviceService deviceService)
		{
			var path = deviceService.DeviceRuntimePlatform != Device.Android
				    ? "Assets"
				    : "";
			switch (deviceService.DeviceRuntimePlatform)
			{
				case Device.Android:
					break;
				case Device.UWP:
				case Device.GTK:
					path += "/";
					break;
				case Device.iOS:
				case Device.macOS:
				case Device.WPF:
					path += @"\";
					break;
			}
			var configurationFile = await fileStorage.ReadAsString($"{path}config.json");

			var configuration = JsonConvert.DeserializeObject<IApplicationConfiguration>(configurationFile);

			//configuration.Platform = platform;

			return configuration;
		}
		public static async Task<IAppCenterConfiguration> GetAppCenterConfiguration(IFileStorage fileStorage, IDeviceService deviceService)
		{
			var configuration = await GetApplicationConfiguration(fileStorage, deviceService);

			return new AppCenterConfiguration((configuration as IAppCenterConfiguration).settings);
		}
		public static async Task<IGitHubConfiguration> GetGitHubConfiguration(IFileStorage fileStorage, IDeviceService deviceService)
		{
			var configuration = await GetApplicationConfiguration(fileStorage, deviceService);

			return new GitHubConfiguration((configuration as IGitHubConfiguration).settings);
		}
	}
}
