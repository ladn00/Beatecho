﻿using System;
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
    }
}
