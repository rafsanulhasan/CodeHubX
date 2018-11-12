﻿
using Tizen.Applications;
using ElmSharp;

namespace CodeHubX.WIndows.WPF
{
	class App : CoreUIApplication
	{
		protected override void OnCreate()
		{
			base.OnCreate();
			Initialize();
		}

		void Initialize()
		{
			Window window = new Window("ElmSharpApp")
			{
				AvailableRotations = DisplayRotation.Degree_0 | DisplayRotation.Degree_180 | DisplayRotation.Degree_270 | DisplayRotation.Degree_90
			};
			window.BackButtonPressed += (s, e) =>
			{
				Exit();
			};
			window.Show();

			var box = new Box(window)
			{
				AlignmentX = -1,
				AlignmentY = -1,
				WeightX = 1,
				WeightY = 1,
			};
			box.Show();

			var bg = new Background(window)
			{
				Color = Color.White
			};
			bg.SetContent(box);

			var conformant = new Conformant(window);
			conformant.Show();
			conformant.SetContent(bg);

			var label = new Label(window)
			{
				Text = "Hello, Tizen",
				Color = Color.Black
			};
			label.Show();
			box.PackEnd(label);
		}

		static void Main(string[] args)
		{
			Elementary.Initialize();
			Elementary.ThemeOverlay();
			App app = new App();
			app.Run(args);
		}
	}
}
