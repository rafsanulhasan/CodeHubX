
using CodeHubX.Services;
using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;

namespace CodeHubX.Apple.Mobile
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register(nameof(AppDelegate))]
	public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Xamarin.Forms.Forms.Init();
			LoadApplication(new App(new UniversaliOSInitializer()));

			return base.FinishedLaunching(app, options);
		}
	}

	public class UniversaliOSInitializer
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
