namespace MiniServer.Models
{
    public partial class Tracks
    {
        public int Id { get; set; }
        public int Albumid { get; set; }
        public string Name { get; set; }
        public int Tracknumber { get; set; }
        public string Filepath { get; set; }
        public int Duration { get; set; }

        public virtual Albums Album { get; set; }
    }
}
