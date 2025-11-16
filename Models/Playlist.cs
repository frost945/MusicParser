using Avalonia.Media.Imaging;
using MusicParserDesktop;
using System.Collections.Generic;

namespace MusicParser.Models
{
    internal class Playlist
    {
        public string? Title { get; set; } = string.Empty;
        public Bitmap? Avatar { get; set; }
        public string? DescriptionPrimary { get; set; } = string.Empty;
        public string? DescriptionSecondary { get; set; } = string.Empty;
        public List<Song> Songs { get; set; } = new();
    }
}
