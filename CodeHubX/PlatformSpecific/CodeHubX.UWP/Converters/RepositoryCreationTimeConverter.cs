using Octokit;
using System;
using Windows.UI.Xaml.Data;


namespace CodeHubX.UWP.Converters
{
	/// <summary>
	/// A simple converter that returns the formatted creation time for a repository
	/// </summary>
	public class RepositoryCreationTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
			=> (value as Repository)?.CreatedAt.ToString("dd MMM yyyy", null)
			?? string.Empty;

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> throw new NotImplementedException();
	}
}
