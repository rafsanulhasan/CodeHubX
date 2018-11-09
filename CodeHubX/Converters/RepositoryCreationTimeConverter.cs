using Octokit;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CodeHubX.Converters
{
	/// <summary>
	/// A simple converter that returns the formatted creation time for a repository
	/// </summary>
	public class RepositoryCreationTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> (value as Repository)?.CreatedAt.ToString("dd MMM yyyy", null)
			?? string.Empty;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
