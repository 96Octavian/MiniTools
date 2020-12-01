using MiniServer.Models;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace MiniServer.ViewModels
{
    public class Artist
    {
        [JsonPropertyName("id")]
        public int ID { get; }

        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("albums")]
        public ImmutableList<Album> Albums { get; }

        public Artist(Artists artist)
        {
            ID = artist.Id;
            Name = artist.Name;
            if (artist.Albums is not null)
                Albums = ImmutableList.Create(artist.Albums.Select(album => new Album(album)).ToArray());
        }
    }
}
