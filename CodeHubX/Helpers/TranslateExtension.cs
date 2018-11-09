using CodeHubX.Converters;
using CodeHubX.Services;
using System;
using System.Globalization;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Helpers
{
	public static class L10Resources
	{
		public const string AppDisplayName = "appName";
	}

	[ContentProperty(nameof(ResourceName))]
	public sealed class TranslateExtension
		: IMarkupExtension,
		  IMarkupExtension<string>
	{
		private readonly CultureInfo ci = null;
		private const string ResourceId = "CodeHubX.Strings.LangResource";
		private const string SelfPath = ".";
		private static readonly Lazy<ResourceManager> ResMgr = new Lazy<ResourceManager>(
		  () => new ResourceManager(ResourceId, typeof(TranslateExtension).Assembly));

		public string Text { get; set; }
		public string ResourceName { get; set; }
		public string Comment { get; set; }


		public TranslateExtension()
			=> ci = DependencyService.Get<ILocalizer>().GetCurrentCultureInfo();

		public string ProvideValue(IServiceProvider serviceProvider)
		{
			string GetLocalizedValue(string resourceName, string comment)
			{
				if (StringHelper.IsNullOrEmptyOrWhiteSpace(resourceName))
					throw new ArgumentNullException(nameof(resourceName));
				if (StringHelper.IsNullOrEmptyOrWhiteSpace(comment))
					return L10n.Localize(resourceName, "");
				else
					return L10n.Localize(resourceName, comment);
			}

			var translation = string.Empty;
			if (StringHelper.IsNullOrEmptyOrWhiteSpace(ResourceName))
				ResourceName = Text;

			translation = GetLocalizedValue(ResourceName, Comment);
			if (StringHelper.IsNullOrEmptyOrWhiteSpace(translation))
			{
#if DEBUG
				throw new ArgumentException(
				    $"Key '{ResourceName}' was not found in resources '{ResourceId}' for culture '{ci.Name}'.",
				    "Text");
#else
                translation = ResourceName; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
			}
			if (!StringHelper.IsNullOrEmptyOrWhiteSpace(Text))
			{
				var texts = Text.Split(',');
				if (texts.Length >= 1)
					translation = string.Format(translation, texts);
			}
			return translation;
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
			=> (this as IMarkupExtension<string>).ProvideValue(serviceProvider);
	}
}
