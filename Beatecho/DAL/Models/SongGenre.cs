﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class SongGenre
    {
        public int SongId { get; set; }
        public virtual Song Song { get; set; }

        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
