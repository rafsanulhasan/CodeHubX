using CodeHubX.Services;
using Prism;
using Prism.Ioc;
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
			LoadApplication(new CodeHubX.App(new WpfInitializer()));
			//Title = Localization.Localize("appName");
			Title = "CodeHubX";
		}
	}

	public class WpfInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Register any platform specific implementations
			containerRegistry.RegisterSingleton<IFileStorage, FileStorage>();
			containerRegistry.RegisterSingleton<ILocalizer, StringLocalizer>();
		}
	}
}
