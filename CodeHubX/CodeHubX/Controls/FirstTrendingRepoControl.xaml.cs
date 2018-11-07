using Octokit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public sealed partial class FirstTrendingRepoControl : ContentView
	{
		public Repository Repository
		{
			get => (Repository) GetValue(RepositoryProperty);
			set => SetValue(RepositoryProperty, value);
		}

		public static readonly BindableProperty RepositoryProperty =
		  BindableProperty.Create(nameof(Repository), typeof(Repository), typeof(FirstTrendingRepoControl));

		//public FirstTrendingRepoControl()
		//	=> InitializeComponent();
	}
}
