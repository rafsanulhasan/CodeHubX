namespace CodeHubX.Models
{
	public interface IAppCenterSettings
	{
		dynamic android { get; }
		dynamic macOS { get; }
		string uwp { get; }
	}
}
