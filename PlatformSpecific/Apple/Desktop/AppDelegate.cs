using AppKit;
using CodeHubX.Services;
using Foundation;
using Prism;
using Prism.Ioc;

namespace CodeHubX.Apple.OSX
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate 
		//: NSApplicationDelegate
		: Xamarin.Forms.Platform.MacOS.FormsApplicationDelegate
    {
		public override NSWindow MainWindow { get; }

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//

		public override void DidFinishLaunching(NSNotification notification)
		{ 
			Xamarin.Forms.Forms.Init();
			LoadApplication(new App(new MacOSInitializer()));

			base.DidFinishLaunching(notification);
		}
	}

	public class MacOSInitializer
		: IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Register any platform specific implementations
			containerRegistry.RegisterSingleton<IFileStorage, FileStorage>();
			containerRegistry.RegisterSingleton<ILocalizer, StringLocalizer>();
		}
	}
}
