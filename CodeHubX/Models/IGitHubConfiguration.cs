namespace CodeHubX.Models
{
	public interface IGitHubConfiguration
		: IConfiguration
	{
		IGitHubSettings settings { get; }
	}
}
