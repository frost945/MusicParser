using System;
using System.IO;
using System.Threading.Tasks;

namespace MusicParser.Infrastructure
{
    public static class PlaywrightInstaller
    {
        private static readonly string _installFlagPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MusicParser", "playwright.installed");
        public static bool IsPlaywrightInstalled()
        {
            //check the flag that the browsers have already been installed
            return File.Exists(_installFlagPath);
        }

        public static async Task InstallPlaywrightBrowsers()
        {
            try
            {
                Console.WriteLine("Installing Playwright browsers...");

                // launch the installation of browsers via Playwright CLI
                var exitCode = await Task.Run(() => Microsoft.Playwright.Program.Main(new[] { "install" }));

                if (exitCode == 0)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_installFlagPath));
                    File.WriteAllText(_installFlagPath, DateTime.Now.ToString());

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
            }
        }
    }
}
