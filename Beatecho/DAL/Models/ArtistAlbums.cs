using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class ArtistAlbums
    {
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }

        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }
    }
}
