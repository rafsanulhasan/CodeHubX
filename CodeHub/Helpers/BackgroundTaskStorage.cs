using System.Collections.Generic;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace CodeHub.Helpers
{
	public static class BackgroundTaskStorage
	{
		private static IPropertySet Values = ApplicationData.Current.LocalSettings.CreateContainer("BackgroundTaskStorage", ApplicationDataCreateDisposition.Always).Values;

		public static IPropertySet GetValues()
			=> Values;

		public static void PutError(string message)
			=> Values["error"] = message;

		public static string GetError()
			=> Values.ContainsKey("error") ? Values["error"] as string : null;

		public static void PutAnswer(object answer)
		{
			// Clear the message since it was successful
			PutError(null);

			Values["answer"] = answer;
		}

		public static object GetAnswer()
		{
			Values.TryGetValue("answer", out var obj);

			return obj;
		}

		public static IDictionary<string, object> ConvertValueSetToDictionary(ValueSet valueSet)
		{
			var converted = new Dictionary<string, object>();

			valueSet.ForEach(value=>converted[value.Key] = value.Value);

			return converted;
		}

		public static ApplicationDataCompositeValue ConvertValueSetToApplicationDataCompositeValue(ValueSet valueSet)
		{
			var converted = new ApplicationDataCompositeValue();

			valueSet.ForEach(value=>converted[value.Key]=value.Value);

			return converted;
		}
	}
}
