using MiniServer.Models;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace MiniServer.ViewModels
{
    public class Album
    {
        [JsonPropertyName("id")]
        public int ID { get; }

        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("year")]
        public int Year { get; }

        [JsonPropertyName("tracks")]
        public ImmutableList<Track> Tracks { get; } = null;

        [JsonPropertyName("artistID")]
        public int ArtistID { get; }

        [JsonPropertyName("artist")]
        public string Artist { get; }

        public Album(Albums album)
        {
            ID = album.Id;
            Name = album.Name;
            if (album.Releasedate.HasValue)
                Year = album.Releasedate.Value.Year;
            if (album.Tracks is not null)
                Tracks = ImmutableList.Create(album.Tracks.Select(track => new Track(track)).ToArray());
            ArtistID = album.Artistid;
            if (album.Artist is not null)
            {
                Artist = album.Artist.Name;
            }
        }
    }
}
