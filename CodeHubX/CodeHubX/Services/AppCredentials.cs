using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace CodeHubX.Services
{
	public class AppCredentials
	{
		private static readonly string AppConfigPath = $"{Path.Combine(Environment.CurrentDirectory, "app.config")}";

		/* These methods get App key and secret from an xml file called app.config. Create your app.config in the following format:
		* 
		*     <?xml version="1.0" encoding="utf-8" ?>
		*       <configuration>
		*          <appSettings>
		*            <add key="AppKey" value="[key here]"/>
		*            <add key="AppSecret" value="[secret here]"/>
		*          </appSettings>
		*        </configuration>
		*/

		public static async Task<string> GetAppKey()
			=> await Task.Run(() =>
			{
				try
				{
					var xml = File.ReadAllText(AppConfigPath);
					var xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(xml);

					var node = xmlDocument
								.DocumentElement
								.SelectSingleNode("./appSettings/add[@key='AppKey']/@value");
					return node?.Value ?? null;
				}
				catch
				{
					return null;
				}
			});


		public static async Task<string> GetAppSecret()
			=> await Task.Run(() =>
			{
				try
				{
					var xml = File.ReadAllText(AppConfigPath);
					var xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(xml);

					var node = xmlDocument
								.DocumentElement
								.SelectSingleNode("./appSettings/add[@key='AppSecret']/@value");

					return node?.Value ?? null;
				}
				catch { return null; }
			});
	}
}
