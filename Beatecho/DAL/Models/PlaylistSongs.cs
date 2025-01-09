using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class PlaylistSongs
    {
        public int PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }

        public int SongId { get; set; }
        public virtual Song Song { get; set; }
    }
}
