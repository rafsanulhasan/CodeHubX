namespace CodeHubX.Models
{
	public class AppCenterConfiguration
		: IAppCenterConfiguration
	{
		public IAppCenterSettings settings { get; set; }

		public AppCenterConfiguration() { }
		public AppCenterConfiguration(IAppCenterSettings appCenterSettings)
			=> settings = appCenterSettings;
	}
}
