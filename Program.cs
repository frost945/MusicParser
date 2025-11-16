using Avalonia;
using System;
using System.Threading.Tasks;
using MusicParser.Infrastructure;

namespace MusicParser;

sealed class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        //check and install Playwright browsers if not installed
        if (!PlaywrightInstaller.IsPlaywrightInstalled())
        {
            await PlaywrightInstaller.InstallPlaywrightBrowsers();
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

