using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Services
{
	public interface ILocalization
	{
		void SetLocale(CultureInfo ci);

		/// <remarks>
		/// Maybe we can cache this info rather than querying every time
		/// </remarks>
		[Obsolete]
		string Locale();

		string Localize(string key, string comment = "");

		ResourceDictionary LocalizeResource(ResourceDictionary resources, params string[] keys);

		void LocalizeResource(ResourceDictionary dic, string key, string comment = null);
	}
}
