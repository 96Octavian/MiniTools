using System;
using System.Collections.Generic;

namespace MiniServer.Models
{
    public class Albums
    {
        public Albums()
        {
            Tracks = new HashSet<Tracks>();
        }

        public int Id { get; set; }
        public int Artistid { get; set; }
        public string Name { get; set; }
        public DateTime? Releasedate { get; set; }
        public string Picture { get; set; }

        public Artists Artist { get; set; }
        public ICollection<Tracks> Tracks { get; protected set; }
    }
}
