namespace CodeHubX.Tizen4.Mobile
{
	internal class Program : Xamarin.Forms.Platform.Tizen.FormsApplication
	{
		protected override void OnCreate()
		{
			base.OnCreate();

			LoadApplication(new App());
		}

		private static void Main(string[] args)
		{
			var app = new Program();
			Xamarin.Forms.Platform.Tizen.Forms.Init(app);
			app.Run(args);
		}
	}
}
