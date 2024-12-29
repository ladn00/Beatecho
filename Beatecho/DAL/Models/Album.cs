using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class Album
    {
        public int Id { get; set; }
        public int ReleaseYear { get; set; }
        public string? Title { get; set; }
        public string Photo { get; set; }

        public virtual ICollection<AlbumSongs> AlbumSongs { get; set; } = new List<AlbumSongs>();
        public virtual ICollection<ArtistAlbums> ArtistAlbums { get; set; } = new List<ArtistAlbums>();
    }
}
