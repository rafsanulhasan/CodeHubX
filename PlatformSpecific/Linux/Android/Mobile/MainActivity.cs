
using Android.App;
using Android.Content.PM;
using Android.OS;
using CodeHubX.Services;
using Prism;
using Prism.Ioc;

namespace CodeHubX.Droid.Mobile
{
	[Activity(Label = "CodeHubX", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;			

			base.OnCreate(savedInstanceState);

			Xamarin.Forms.Forms.Init(this, savedInstanceState);
			LoadApplication(new App(new AndroidInitializer()));
		}
	}

	public class AndroidInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Register any platform specific implementations
			containerRegistry.RegisterSingleton<IFileStorage, FileStorage>();
			containerRegistry.RegisterSingleton<ILocalizer, StringLocalizer>();
		}
	}
}