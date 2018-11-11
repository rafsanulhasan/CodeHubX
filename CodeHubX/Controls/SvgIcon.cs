using SkiaSharp.Extended.Svg;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;

namespace CodeHubX.Controls
{
	public class SvgIcon : Frame
	{
		#region Private Members

		private readonly SKCanvasView _canvasView = new SKCanvasView();

		#endregion

		#region Bindable Properties

		#region ResourceId

		public static readonly BindableProperty ResourceIdProperty = BindableProperty.Create(
		    nameof(ResourceId), typeof(string), typeof(SvgIcon), default(string), propertyChanged: RedrawCanvas);

		public string ResourceId
		{
			get => (string) GetValue(ResourceIdProperty);
			set => SetValue(ResourceIdProperty, value);
		}

		#endregion

		#endregion

		#region Constructor

		public SvgIcon()
		{
			Padding = new Thickness(0);
			BackgroundColor = Color.Transparent;
			HasShadow = false;
			Content = _canvasView;
			_canvasView.PaintSurface += CanvasViewOnPaintSurface;
		}

		#endregion

		#region Private Methods

		private static void RedrawCanvas(BindableObject bindable, object oldvalue, object newvalue)
		{
			var svgIcon = bindable as SvgIcon;
			svgIcon?._canvasView.InvalidateSurface();
		}

		private void CanvasViewOnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
		{
			var canvas = args.Surface.Canvas;
			canvas.Clear();

			if (string.IsNullOrEmpty(ResourceId))
				return;

			using (var stream = GetType().Assembly.GetManifestResourceStream(ResourceId))
			{
				var svg = new SKSvg();
				svg.Load(stream);

				var info = args.Info;
				canvas.Translate(info.Width / 2f, info.Height / 2f);

				var bounds = svg.ViewBox;
				var xRatio = info.Width / bounds.Width;
				var yRatio = info.Height / bounds.Height;

				var ratio = Math.Min(xRatio, yRatio);

				canvas.Scale(ratio);
				canvas.Translate(-bounds.MidX, -bounds.MidY);

				canvas.DrawPicture(svg.Picture);
			}
		}

		#endregion
	}
}
