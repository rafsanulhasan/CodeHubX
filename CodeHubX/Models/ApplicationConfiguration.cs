namespace CodeHubX.Models
{
	public class ApplicationConfiguration
		: IApplicationConfiguration
	{
		IAppCenterSettings IAppCenterConfiguration.settings { get; }
		IGitHubSettings IGitHubConfiguration.settings { get; }
	}
}
