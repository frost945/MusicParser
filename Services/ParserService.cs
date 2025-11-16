using Avalonia.Media.Imaging;
using Microsoft.Playwright;
using MusicParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicParserDesktop
{
    public class ParserService
    {
        internal async Task<Playlist> ParseAmazonMusicAsync(string url)
        {
            Console.WriteLine("ParseHtmlAsync start...\nurl: " + url);

            //parser restriction only for this type URL
            if (!url.Contains("https://music.amazon.com/albums/") && !url.Contains("https://music.amazon.com/playlists/"))
            {
                throw new Exception($"incorrect url format");
            }

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Args = new[] { "--window-position=-32000,-32000" }// temporary solution
            });

            var page = await browser.NewPageAsync();

            page.SetDefaultTimeout(120000);
            page.SetDefaultNavigationTimeout(120000);

            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 60000
            });

            await Task.Delay(3000);

            if (url.Contains("https://music.amazon.com/playlists/"))
            {
                return await ParsePlaylistAsync(url, page);
            }
            else
            {
                return await ParseAlbumAsync(url, page);
            }
        }

        private async Task<Playlist> ParsePlaylistAsync(string url, IPage page)
        {
            try
            {
                var h1 = await page.TextContentAsync("h1[title]");
                Console.WriteLine($"h1[title] found: {h1 ?? "NULL"}");

                var descPrimary = await page.EvalOnSelectorAsync<string>("music-link[title]", "el => el.getAttribute('title')");
                Console.WriteLine("description1: " + descPrimary);

                var descSecondary = await page.EvalOnSelectorAsync<string>(
                    "p.secondaryText music-link[title]",
                    "el => el.getAttribute('title')");
                Console.WriteLine("description2: " + descSecondary);

                Bitmap? avatarBitmap = await ParseAvatar(page);

                var songTitles = await page.EvalOnSelectorAllAsync<string[]>(
                    ".col1 a",
                    "els => els.map(el => el.textContent.trim())");

                var artists = await page.EvalOnSelectorAllAsync<string[]>(
                    ".col2 a",
                    "els => els.map(el => el.textContent.trim())");

                var albums = await page.EvalOnSelectorAllAsync<string[]>(
                    ".col3 a",
                    "els => els.map(el => el.textContent.trim())");

                var durations = await page.EvalOnSelectorAllAsync<string[]>(
                    ".col4",
                    "els => els.map(el => el.textContent.trim())");

                var songs = new List<Song>();

                for (int i = 0; i < songTitles.Length; ++i)
                {
                    var song = new Song
                    {
                        Title = songTitles[i],
                        Artist = artists[i],
                        Album = albums[i],
                        Duration = durations[i],
                    };
                    songs.Add(song);

                    Console.WriteLine($"{i + 1} | title: \"{song.Title}\" | artist: \"{song.Artist}\" | album: \"{song.Album}\" | duration: {song.Duration}");
                }
                return new Playlist
                {
                    Title = h1,
                    Avatar = avatarBitmap,
                    DescriptionPrimary = descPrimary ?? string.Empty,
                    DescriptionSecondary = descSecondary ?? string.Empty,
                    Songs = songs,
                };
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Parsing failed: {ex.Message}");
                throw new Exception($"Parsing failed: {ex.Message}");
            }
        }

        private async Task<Playlist> ParseAlbumAsync(string url, IPage page)
        {
            try
            {
                var h1 = await page.TextContentAsync("h1[title]");
                Console.WriteLine("h1: " + h1);

                var descPrimary = await page.EvalOnSelectorAsync<string>("music-link.primary a", "el => el.textContent.trim()");
                Console.WriteLine("description1: " + descPrimary);

                var descSecondary = await page.EvalOnSelectorAsync<string>(
                "p.tertiaryText music-link span",
                "el => el.textContent.trim()");
                Console.WriteLine("description2: " + descSecondary);

                Bitmap? avatarBitmap = await ParseAvatar(page);

                var songTitles = await page.EvalOnSelectorAllAsync<string[]>(
               ".col1 a",
               "els => els.map(el => el.textContent.trim())");

                var artists = await page.EvalOnSelectorAllAsync<string[]>(
                    ".col3 a",
                    "els => els.map(el => el.textContent.trim())");

                // in albums artist info can be missing
                if (artists == null || artists.Length == 0)
                {
                    artists = Enumerable.Repeat("", songTitles.Length).ToArray();
                }

                var durations = await page.EvalOnSelectorAllAsync<string[]>(
                ".col4",
                "els => els.map(el => el.textContent.trim())");

                var songs = new List<Song>();

                for (int i = 0; i < songTitles.Length; ++i)
                {
                    var song = new Song
                    {
                        Title = songTitles[i],
                        Artist = artists[i],
                        Duration = durations[i],
                    };
                    songs.Add(song);

                    Console.WriteLine($"{i + 1} | title: \"{song.Title}\" | duration: {song.Duration}");
                }

                return new Playlist
                {
                    Title = h1,
                    Avatar = avatarBitmap,
                    DescriptionPrimary = descPrimary,
                    DescriptionSecondary = descSecondary,
                    Songs = songs,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parsing failed: {ex.Message}");
                throw new Exception($"Parsing failed: {ex.Message}");
            }
        }

        private async Task<Bitmap?> ParseAvatar(IPage page)
        {
            var imgs = await page.EvalOnSelectorAllAsync<string[]>(
                  "img[role='none']",
                  "els => els.map(el => el.getAttribute('data-src') || el.getAttribute('src') || '')"
              );

            var avatarUrl = imgs.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

            if (string.IsNullOrWhiteSpace(avatarUrl))
            {
                Console.WriteLine("avatar not found");
                return null;
            }
            else
            {
                Bitmap? avatarBitmap = await DownloadImageAsync(avatarUrl);

                Console.WriteLine("avatar bitmap created: " + (avatarBitmap != null));
                
                return avatarBitmap;
            }
        }

        private async Task<Bitmap?> DownloadImageAsync(string imageUrl)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                var cacheDir = Path.Combine(Directory.GetCurrentDirectory(), "ImageCache");
                if (!Directory.Exists(cacheDir))
                    Directory.CreateDirectory(cacheDir);

                var fileName = $"{Guid.NewGuid()}.jpg";
                var fullPath = Path.Combine(cacheDir, fileName);

                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                await File.WriteAllBytesAsync(fullPath, imageBytes);

                return new Bitmap(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image: {ex.Message}");
                return null;
            }
        }
    }
}