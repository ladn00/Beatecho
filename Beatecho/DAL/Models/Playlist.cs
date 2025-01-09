using System;
using System.Collections.Generic;

namespace Beatecho.DAL.Models;

public class Playlist
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Photo { get; set; }

    public bool? IsPublic { get; set; }

    public virtual ICollection<PlaylistSongs> PlaylistSongs { get; set; } = new List<PlaylistSongs>();
    public virtual ICollection<PlaylistUsers> PlaylistUsers { get; set; } = new List<PlaylistUsers>();
}
