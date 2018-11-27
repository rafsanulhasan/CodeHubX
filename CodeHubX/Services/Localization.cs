using CodeHubX.Helpers;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;

namespace CodeHubX.Services
{
	public class Localization
		: ILocalization
	{
		private const string ResourceId = "CodeHubX.Strings.LangResource";
		private readonly Lazy<ResourceManager> ResMgr
			= new Lazy<ResourceManager>(() =>
				new ResourceManager(ResourceId, IntrospectionExtensions.GetTypeInfo(typeof(Localization)).Assembly));

		private ILocalizer _Localizer;

		public Localization(ILocalizer localizer)
		{
			_Localizer=localizer;
		}

		public void SetLocale(CultureInfo ci)
			=> _Localizer.SetLocale(ci);

		/// <remarks>
		/// Maybe we can cache this info rather than querying every time
		/// </remarks>
		[Obsolete]
		public string Locale()
			=> _Localizer.GetCurrentCultureInfo().ToString();

		public string Localize(string key, string comment = "")
		{
			//var netLanguage = Locale ();

			// Platform-specific
			Debug.WriteLine("Localize " + key);
			var result = ResMgr.Value.GetString(key, _Localizer.GetCurrentCultureInfo());

			if (StringHelper.IsNullOrEmptyOrWhiteSpace(result))
			{
				result = key; // HACK: return the key, which GETS displayed to the user
			}
			return result;
		}

		public ResourceDictionary LocalizeResource(ResourceDictionary resources, params string[] keys)
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

		public void LocalizeResource(ResourceDictionary dic, string key, string comment = null)
			=> dic[key] = Localize(key, comment);
	}
}
