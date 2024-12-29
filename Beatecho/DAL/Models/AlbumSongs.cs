using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class AlbumSongs
    {
        public int SongId { get; set; }
        public virtual Song Song { get; set; }

        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }

        public int TrackNum { get; set; }
    }
}
