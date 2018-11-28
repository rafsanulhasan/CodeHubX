using Prism;
using Prism.Ioc;

namespace CodeHubX.Tizen
{
	using Services;

	public class TizenInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Register any platform specific implementations
			containerRegistry.RegisterSingleton<IFileStorage, FileStorage>();
			containerRegistry.RegisterSingleton<ILocalizer, StringLocalizer>();
		}
	}
}
