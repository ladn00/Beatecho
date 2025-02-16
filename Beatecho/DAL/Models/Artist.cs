using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string bio { get; set; }
        public string? Photo { get; set; }

        public virtual ICollection<ArtistAlbums> ArtistAlbums { get; set; } = new List<ArtistAlbums>();
    }
}
