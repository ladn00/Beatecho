using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Beatecho.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private Player player;
        public ObservableCollection<Album> Albums { get; set; }

        public MainPageViewModel()
        {
            PlayPauseAlbumCommand = new RelayCommand<Album>(PlayPauseAlbum);
            OpenAlbumCommand = new RelayCommand<object>(OpenAlbum);
            player = PlayerViewModel.player;
            Albums = new ObservableCollection<Album>(LoadAlbums());
        }

        public ICommand PlayPauseAlbumCommand { get; }
        public ICommand OpenAlbumCommand { get; }

        private IEnumerable<Album> LoadAlbums()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Albums.ToList();
            }
        }

        private void OpenAlbum(object parameter)
        {
            if (parameter is Album selectedAlbum)
            {
                var albumPage = new AlbumPage(selectedAlbum);

                Views.Wins.UserWindow.frame.NavigationService.Navigate(albumPage);
            }
        }

        private void PlayPauseAlbum(Album album)
        {
            if (player == null || album == null)
            {
                return;
            }

            List<Song> songs = new List<Song>();

            if (player.IsPlaying && player.AlbumPlaylist == album)
            {
                player.Pause();
                return;
            }
            else if (!player.IsPlaying && player.AlbumPlaylist == album)
            {
                player.Play();
                return;
            }

            songs = GetSongsFromAlbum(album);

            player.AlbumPlaylist = album;
            player.SetQueue(songs);
            player.SetSong();
            player.Play();
        }

        public List<Song> GetSongsFromAlbum(Album album)
        {
            List<Song> songs = new List<Song>();

            using (ApplicationContext db = new ApplicationContext())
            {
                if (album != null)
                {
                    db.Entry(album)
                        .Collection(a => a.AlbumSongs) // Явная загрузка AlbumSongs
                        .Load();

                    var albumSongs = album.AlbumSongs.ToList();

                    foreach (AlbumSongs s in albumSongs)
                    {
                        songs.Add(s.Song);
                    }
                }
            }
            return songs;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
