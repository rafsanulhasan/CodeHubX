namespace CodeHubX.Tizen.Watch
{
	class Program : Xamarin.Forms.Platform.Tizen.FormsApplication
	{
		protected override void OnCreate()
		{
			base.OnCreate();

			LoadApplication(new App());
		}

		static void Main(string[] args)
		{
			var app = new Program();
			Xamarin.Forms.Platform.Tizen.Forms.Init(app);
			app.Run(args);
		}
	}
}
