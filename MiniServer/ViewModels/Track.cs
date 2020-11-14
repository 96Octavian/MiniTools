using MiniServer.Models;
using System;
using System.Text.Json.Serialization;

namespace MiniServer.ViewModels
{
    public class Track
    {
        [JsonPropertyName("id")]
        public int ID { get; }

        [JsonPropertyName("title")]
        public string Title { get; }

        [JsonPropertyName("artistID")]
        public int ArtistID { get; }

        [JsonPropertyName("artist")]
        public string Artist { get; }

        [JsonPropertyName("albumID")]
        public int AlbumID { get; }

        [JsonPropertyName("album")]
        public string Album { get; }

        [JsonPropertyName("year")]
        public int Year { get; }

        [JsonPropertyName("tracknumber")]
        public int Tracknumber { get; } = 0;

        [JsonPropertyName("duration")]
        public TimeSpan Duration { get; }

        public Track(Tracks track)
        {
            // TODO: Duration
            ID = track.Id;
            Title = track.Name;
            Tracknumber = track.Tracknumber;
            AlbumID = track.Albumid;
            if (track.Album is not null)
            {
                Album = track.Album.Name;
                if (track.Album.Releasedate.HasValue)
                    Year = track.Album.Releasedate.Value.Year;
                if (track.Album.Artist is not null)
                {
                    ArtistID = track.Album.Artistid;
                    Artist = track.Album.Artist.Name;
                }
            }
        }
    }
}
