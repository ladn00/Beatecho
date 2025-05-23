﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Salt { get; set; }
        public int? UserTypeId { get; set; }

        public virtual UserType UserType { get; set; }
        public virtual ICollection<PlaylistUsers> PlaylistUsers { get; set; } = new List<PlaylistUsers>();
        public virtual ICollection<FavoriteTracks> FavoriteTracks { get; set; } = new List<FavoriteTracks>();
    }
}
