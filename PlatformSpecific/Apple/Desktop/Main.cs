using AppKit;
using MacOS=Xamarin.Forms.Platform.MacOS;

namespace CodeHubX.Apple.OSX
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
