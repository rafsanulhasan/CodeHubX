
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace CodeHubX.Droid
{
	[Activity(Label = "@string/appName", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			//TabLayoutResource = Android.Resource.Layout.Tabbar;
			//ToolbarResource = Android.Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);
			Xamarin.Forms.Forms.Init(this, savedInstanceState);
			LoadApplication(new App());
		}
	}
}