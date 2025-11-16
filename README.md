# Music Parser

Avalonia desktop application for parsing music playlists

## Features
- Playlist parsing, retrieving playlist title, description, and avatar data.
- For each song, the song title, artist name, album title, and duration are displayed.

## Technologies
- .NET 9.0 SDK
- Avalonia
- Playwright 

## Setup Instructions
1. Clone the repository
2. Open terminal in project folder
3. Run: `dotnet restore`
4. Run: `dotnet run`
- if browsers not found, run: `pwsh bin/Debug/net9.0/playwright.ps1 install`

## Project Structure
- Views/ - UI components (XAML)
- ViewModels/ - Business logic
- Models/ - Data models
- Services/ - Parser service
- Infrastructure/ - Installation Playwright

## Dependencies
- Avalonia UI
- Playwright for browser automation

## Notes
- The first launch may take some time to load Playwright browsers (1-2 min)
- Application parses music playlists from supported websites
- URL requirements: The URL must be in the format https://music.amazon.com/playlists/xxxxxxxxxx, https://music.amazon.com/albums/xxxxxxxxxx



