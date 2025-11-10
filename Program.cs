using Avalonia;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicParser;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task Main(string[] args)
    {
       //Install Playwright browsers on first launch
        await InstallPlaywrightBrowsers();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static async Task InstallPlaywrightBrowsers()
    {
        try
        {
            Console.WriteLine("Installing Playwright browsers...");

            // Запускаем установку браузеров через Playwright CLI
            var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });

            if (exitCode == 0)
            {
                Console.WriteLine("Playwright browsers installed successfully");
            }
            else
            {
                Console.WriteLine($"Playwright installation completed with code: {exitCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Playwright installation note: {ex.Message}");
            // Не прерываем работу - браузеры могут быть уже установлены
        }
    }
}

