using Beatecho.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Beatecho.DAL.Models;

public class Playlist
{
    [Key]
    public int Id { get; set; }

    public string? Title { get; set; } = "Плейлист";

    public string? Photo { get; set; }

    public bool? IsPublic { get; set; } = false;

    public virtual ICollection<PlaylistSongs> PlaylistSongs { get; set; } = new List<PlaylistSongs>();
    public virtual ICollection<PlaylistUsers> PlaylistUsers { get; set; } = new List<PlaylistUsers>();


    public virtual string PlaylistPhoto
    {
        get
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string projectDir = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.FullName;

            if (projectDir != null)
            {
                if (Photo != null) 
                {
                    string fullPath = Path.Combine(projectDir, "imgs", "Playlists", Photo);
                    return fullPath;
                }
                return null; 
            }
            else
            {
                return null;
            }
        }
    }
}
