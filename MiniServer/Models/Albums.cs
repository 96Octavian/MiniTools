using System;
using System.Collections.Generic;

namespace MiniServer.Models
{
    public partial class Albums
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

        public virtual Artists Artist { get; set; }
        public virtual ICollection<Tracks> Tracks { get; set; }
    }
}
