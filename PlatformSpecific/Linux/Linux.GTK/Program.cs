using CodeHubX.Services;
using Prism;
using Prism.Ioc;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace CodeHubX.Linux
{
	public class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Gtk.Application.Init();
			Forms.Init();

			var app = new App(new LinuxInitializer());
			var window = new FormsWindow();
			window.LoadApplication(app);
			window.SetApplicationTitle("CodeHubX");
			window.Show();

			Gtk.Application.Run();
		}
	}

	public class LinuxInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Register any platform specific implementations
			containerRegistry.RegisterSingleton<ILocalizer, StringLocalizer>();
			containerRegistry.RegisterSingleton<IFileStorage, FileStorage>();
		}
	}
}