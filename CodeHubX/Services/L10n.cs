using CodeHubX.Helpers;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;

namespace CodeHubX.Services
{
	public class L10n
	{
		private const string ResourceId = "CodeHubX.Strings.LangResource";
		private static readonly Lazy<ResourceManager> ResMgr
			= new Lazy<ResourceManager>(() =>
				new ResourceManager(ResourceId, IntrospectionExtensions.GetTypeInfo(typeof(L10n)).Assembly));

		public static void SetLocale(CultureInfo ci)
			=> DependencyService.Get<ILocalizer>().SetLocale(ci);

		/// <remarks>
		/// Maybe we can cache this info rather than querying every time
		/// </remarks>
		[Obsolete]
		public static string Locale()
			=> DependencyService.Get<ILocalizer>().GetCurrentCultureInfo().ToString();

		public static string Localize(string key, string comment = "")
		{
			//var netLanguage = Locale ();

			// Platform-specific
			Debug.WriteLine("Localize " + key);
			var result = ResMgr.Value.GetString(key, DependencyService.Get<ILocalizer>().GetCurrentCultureInfo());

			if (StringHelper.IsNullOrEmptyOrWhiteSpace(result))
			{
				result = key; // HACK: return the key, which GETS displayed to the user
			}
			return result;
		}

		public static ResourceDictionary LocalizeResource(ResourceDictionary resources, params string[] keys)
		{

			var dic = new ResourceDictionary();
			resources.ForEach(d =>
			{
				dic.Add(d.Key, d.Value);
				if (d.Value is string)
				{
					if (!StringHelper.IsNullOrEmptyOrWhiteSpace(d.Key) &&(keys == null || keys.Length == 0))
						LocalizeResource(dic, d.Key);
					else
					{
						keys.ForEach(key => LocalizeResource(dic, key));
					}
				}
			});
			return dic;
		}

		public static void LocalizeResource(ResourceDictionary dic, string key, string comment = null)
			=> dic[key] = Localize(key, comment);
	}
}
