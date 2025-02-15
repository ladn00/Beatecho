using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class FavoriteTracks
    {
        public int SongId { get; set; }
        public virtual Song Song { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
