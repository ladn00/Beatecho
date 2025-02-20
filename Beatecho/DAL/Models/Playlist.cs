using Beatecho.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            return $"pack://application:,,,/imgs/Playlists/{Photo}";
        }
    }
    /*public ICommand OpenPlaylistCommand { get; set; }

    public Playlist()
    {
        OpenPlaylistCommand = new RelayCommand(OpenPlaylist);
    }

    private void OpenPlaylist()
    {
        // Логика открытия страницы плейлиста
        var playlistPage = new Views.Pages.PlaylistPage(this);
        Views.Wins.UserWindow.frame.NavigationService.Navigate(playlistPage);
    }*/
}
