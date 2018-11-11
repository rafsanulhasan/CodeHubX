using Xamarin.Forms;

namespace CodeHubX.Controls
{
	public class HtmlToRtfConverter
	{
		// Getter and Setter
		public static string GetHtmlString(BindableObject obj)
			=> (string) obj.GetValue(HtmlStringProperty);

		public static void SetHtmlString(BindableObject obj, string value)
			=> obj.SetValue(HtmlStringProperty, value);

		public static readonly BindableProperty HtmlStringProperty =
		    BindableProperty.CreateAttached(
			    "HtmlString",
			    typeof(string),
			    typeof(HtmlToRtfConverter),
			    null,
			    BindingMode.TwoWay,
			    null,
			    propertyChanged: (obj, oldValue, newValue) => OnHtmlChanged(obj, oldValue, newValue));


		private static void OnHtmlChanged(BindableObject sender, object oldValue, object newValue)
		{
			if (sender is WebView wv && newValue != null)
			{
				wv.SetValue(HtmlStringProperty, newValue);
			}
		}
	}
}
