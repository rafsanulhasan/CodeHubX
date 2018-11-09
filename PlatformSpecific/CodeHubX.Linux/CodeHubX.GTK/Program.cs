using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace CodeHubX.GTK
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Gtk.Application.Init();
            Forms.Init();

            var app = new App();
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("CodeHubX");
            window.Show();

            Gtk.Application.Run();
        }
    }
}