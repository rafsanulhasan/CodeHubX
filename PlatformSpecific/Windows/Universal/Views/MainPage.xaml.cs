using CodeHubX.Services;
using Prism;
using Prism.Ioc;

namespace CodeHubX.UWP.Views
{
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();

			LoadApplication(new CodeHubX.App(new UwpInitializer()));
		}

		public class UwpInitializer : IPlatformInitializer
		{
			public void RegisterTypes(IContainerRegistry containerRegistry)
			{ 
				// Register any platform specific implementations
				containerRegistry.RegisterSingleton<IFileStorage, FileStorage>();
				containerRegistry.RegisterSingleton<ILocalizer, StringLocalizer>();
			}
		}
	}
}
