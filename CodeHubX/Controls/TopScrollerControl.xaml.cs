using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public sealed partial class TopScrollerControl : ContentView, IDisposable
	{
		/// <summary>
		/// Gets the duration of the fade in/out animation for the control
		/// </summary>
		private static readonly int AnimationDuration = 200;
		
		public TopScrollerControl()
		{
			//this.SetVisualOpacity(0);
			//IsHitTestVisible = false;
			var tapGestureRecognizer=new TapGestureRecognizer();			
			tapGestureRecognizer.Tapped += TopScrollerHandleControl_OnTapped;
			GestureRecognizers.Add(tapGestureRecognizer);
		}

		public void Dispose()
		{
			if (_RelatedScrollView != null)
			{
				_RelatedScrollView.LayoutChanged -= RelatedScrollView_LayoutChanged;
				_RelatedScrollView = null;
			}
		}

		// The ScrollViewer in use
		private ScrollView _RelatedScrollView;

		/// <summary>
		/// Gets or sets the minimum vertical offset before the control is shown
		/// </summary>
		public double VerticalOffsetThreshold { get; set; } = 200;

		/// <summary>
		/// Binds the AutoHideCanvas to the ScrollViewer contained inside a given DependencyObject
		/// </summary>
		/// <param name="parentObject">The object that contains the ScrollViewer</param>
		public void InitializeScrollViewer(BindableObject parentObject)
		{
			if (!IsVisible)
			{
				return;
			}

			////var scrollView = parentObject.FindChild<ScrollView>() ?? throw new ArgumentException("The BindableObject doesn't contain a ScrollView");
			//if (_RelatedScrollView != null)
			//{
			//	_RelatedScrollView.LayoutChanged -= RelatedScrollView_LayoutChanged;
			//}
			//_RelatedScrollView = scrollView;
			//_RelatedScrollView.LayoutChanged += RelatedScrollView_LayoutChanged;
		}

		private DateTime? _LastAnimationStartTime;

		private bool _ButtonShown;

		private void RelatedScrollView_LayoutChanged(object sender, EventArgs args)
		{
			//if (sender.To<ScrollView>().VerticalOffset >= VerticalOffsetThreshold &&
			//    !_ButtonShown &&
			//    (_LastAnimationStartTime == null || DateTime.Now.Subtract(_LastAnimationStartTime.Value).TotalMilliseconds > AnimationDuration))
			//{
			//	_LastAnimationStartTime = DateTime.Now;
			//	_ButtonShown = true;
			//	this.StartXAMLTransformFadeSlideAnimation(null, 1, TranslationAxis.Y, 20, 0, 200, null, null, EasingFunctionNames.SineEaseOut,
			//	    () => IsHitTestVisible = true);
			//}
			//else if (sender.To<ScrollView>().VerticalOffset < VerticalOffsetThreshold &&
			//    _ButtonShown &&
			//    (_LastAnimationStartTime == null || DateTime.Now.Subtract(_LastAnimationStartTime.Value).TotalMilliseconds > AnimationDuration))
			//{
			//	_LastAnimationStartTime = DateTime.Now;
			//	_ButtonShown = false;
			//	IsHitTestVisible = false;
			//	this.StartXAMLTransformFadeSlideAnimation(null, 0, TranslationAxis.Y, 0, 20, 200, null, null, EasingFunctionNames.SineEaseOut);
			//}
		}

		/// <summary>
		/// Raised whenever the user taps on the control to request a scrolling to the top
		/// </summary>
		public event EventHandler TopScrollingRequested;

		private void TopScrollerHandleControl_OnTapped(object sender, EventArgs args)
		{
			if (!_ButtonShown)
			{
				return;
			}

			_LastAnimationStartTime = DateTime.Now;
			_ButtonShown = false;
			//IsHitTestVisible = false;
			//this.StartXAMLTransformFadeSlideAnimation(null, 0, TranslationAxis.Y, 0, 20, 200, null, null, EasingFunctionNames.SineEaseOut);
			TopScrollingRequested?.Invoke(this, EventArgs.Empty);
		}
	}
}
