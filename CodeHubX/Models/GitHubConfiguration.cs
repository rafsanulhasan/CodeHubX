namespace CodeHubX.Models
{
	public class GitHubConfiguration
		: IGitHubConfiguration
	{
		public IGitHubSettings settings { get; set; }
		public GitHubConfiguration() { }
		public GitHubConfiguration(IGitHubSettings gitHubSettings)
			=> settings = gitHubSettings;
	}
}
