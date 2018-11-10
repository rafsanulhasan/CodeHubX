
using Android.App;
using Android.OS;
using Android.Support.Wearable.Activity;
using Android.Widget;

namespace CodeHubX.Droid.Watch
{
	[Activity(Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity : WearableActivity
	{
		private TextView textView;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.activity_main);

			textView = FindViewById<TextView>(Resource.Id.text);
			SetAmbientEnabled();
		}
	}
}


