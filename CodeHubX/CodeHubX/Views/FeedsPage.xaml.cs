using CodeHubX.ViewModels;
using Octokit;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FeedsPage : ContentPage
	{
		private FeedViewmodel _vm;

		public FeedsPage()
		{
			InitializeComponent();
			BindingContext = _vm = new FeedViewmodel();

			DateTimeOffset now = DateTimeOffset.Now,
						createdAt = now - new TimeSpan(11, 11, 11),
						updatedAt = now - new TimeSpan(7, 7, 7),
						forkedAt = now - new TimeSpan(11, 11, 11),
						pushedAt = now - new TimeSpan(5, 5, 5);

			var plan = new Plan(3, "a", 3, 3, "a@a.com");
			var repoPermission = new RepositoryPermissions(true, true, true);
			var user = new User(
							"A",
							"A",
							"a",
							3,
							"6",
							createdAt,
							updatedAt,
							3, "a@a.com",
							3,
							4,
							true,
							"A",
							3,
							1,
							"Dhaka",
							"rafsan",
							"rafsanul hasan",
							"a",
							3,
							plan,
							3,
							3,
							3,
							"a",
							repoPermission,
							true,
							"A",
							null
					);

			var licenseMetadata = new LicenseMetadata("a", "A", "mit", "a", "a", true);

			var repo = new Repository(
						"A",
						"a",
						"a",
						"a",
						"a",
						"a",
						"A",
						1L,
						"a",
						user,
						"a",
						"aa",
						"aaa",
						"a",
						"en-US",
						false,
						false,
						3,
						3,
						"dev",
						3,
						pushedAt,
						createdAt,
						updatedAt,
						repoPermission,
						null,
						null,
						licenseMetadata,
						true,
						true,
						true,
						true,
						50,
						3,
						true,
						true,
						true,
						false);

			var org = new Organization("a", "a", "a", 5, "a", createdAt, 30, "a@a.com", 50, 50, true, "a", 3, 1, "a", "Dhaka", "sics", "6", 3, plan, 3, 3, 3, "a", "A");
			_vm.Events = new ObservableCollection<Activity>()
			{
				new Activity(
					"A",
					true,
					repo,
					user,
					org,
					createdAt,
					"1",
					new ActivityPayload(repo, user, new InstallationId(3)))
			};

			foreach (var item in _vm.ToolbarItems)
			{
				ToolbarItems.Add(item);
			}
		}
		public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{

		}
	}
}