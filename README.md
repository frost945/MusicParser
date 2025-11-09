
# Music Parser Desktop

Avalonia desktop application for parsing music playlists

## Features
- Playlist parsing, retrieving playlist title, description, and avatar data.
- For each song, the song title, artist name, album title, and duration are displayed.

## Technologies
- .NET 9.0 SDK
- Avalonia
- Playwright 

## Setup Instructions

# Local Development
1. Clone the repository
2. Open terminal in project folder
3. Run: `dotnet restore`
4. Run: `dotnet run`

## Project Structure
- Views/ - UI components (XAML)
- ViewModels/ - Business logic
- Models/ - Data models
- Services/ - Parser service

## Dependencies
- Avalonia UI
- Playwright for browser automation

## Notes
- First run may take time to download Playwright browsers
- Application parses music playlists from supported websites
- URL requirements: The URL must be in the format https://music.amazon.com/playlists/xxxxxxxxxx



