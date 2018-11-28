namespace CodeHubX.Models
{
	public interface IAppCenterConfiguration
		: IConfiguration
	{
		IAppCenterSettings settings { get; }
	}
}
