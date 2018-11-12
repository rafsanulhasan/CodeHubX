using Octokit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeHubX.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public sealed partial class CommentListItem : ContentView
	{
		public static BindableProperty DataContextProperty = BindableProperty.Create(nameof(DataContextProperty), typeof(IssueComment), typeof(CommentListItem));

		public IssueComment IssueComment
		{
			get => GetValue(DataContextProperty) as IssueComment;
			set => SetValue(DataContextProperty, value);
		}

		public CommentListItem() =>
			//InitializeComponent();
			Resources.Add(nameof(Converters.TimeAgoConverter), new Converters.TimeAgoConverter());
	}
}
