using System.Collections.Generic;

namespace MiniServer.Models
{
    public class Artists
    {
        public Artists()
        {
            Albums = new HashSet<Albums>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }

        public virtual ICollection<Albums> Albums { get; protected set; }
    }
}
