using CodeHubX.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

namespace CodeHubX.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : FormsApplicationPage
	{
		public MainWindow()
			: base()
		{
			InitializeComponent();
			Forms.Init();
			LoadApplication(new CodeHubX.App());
			Title = L10n.Localize("appName");
		}
	}
}
