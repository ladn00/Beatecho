using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class PlaylistUsers
    {
        public int PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
