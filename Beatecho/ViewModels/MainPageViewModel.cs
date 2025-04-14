using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;

namespace Beatecho.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private Player player;
        private ObservableCollection<Album> _albums;
        private ObservableCollection<Playlist> _playlist;

        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set
            {
                _albums = value;
                OnPropertyChanged(nameof(Albums));
            }
        }

        public ObservableCollection<Playlist> Playlists
        {
            get => _playlist;
            set
            {
                _playlist = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        private ObservableCollection<Recommendation> _recommendations;
        public ObservableCollection<Recommendation> Recommendations
        {
            get => _recommendations;
            set
            {
                _recommendations = value;
                OnPropertyChanged(nameof(Recommendations));
            }
        }

        public MainPageViewModel()
        {
            PlayPauseAlbumCommand = new RelayCommand<Album>(PlayPauseAlbum);
            PlayPauseRecommendationCommand = new RelayCommand<Recommendation>(PlayPauseRecommendation);
            OpenAlbumCommand = new RelayCommand<object>(OpenAlbum);
            PlayPausePlaylistCommand = new RelayCommand<Playlist>(PlayPausePlaylist);
            OpenPlaylistCommand = new RelayCommand<object>(OpenPlaylist);
            OpenRecommendationCommand = new RelayCommand<object>(OpenRecommendation);
            player = PlayerViewModel.player;
            Albums = new ObservableCollection<Album>(LoadAlbums());
            Playlists = new ObservableCollection<Playlist>(LoadPlaylists());
            Recommendations = new() { new Recommendation { Id = 1, Title = "Личная подборка", Photo = "https://ff771a7312a3.hosting.myjino.ru/Covers/recom.jpg" }  };
        }

        public ICommand PlayPauseAlbumCommand { get; }
        public ICommand OpenAlbumCommand { get; }

        public ICommand PlayPausePlaylistCommand { get; }
        public ICommand OpenPlaylistCommand { get; }
        public ICommand OpenRecommendationCommand { get; }
        public ICommand PlayPauseRecommendationCommand { get; }

        private void OpenRecommendation(object parameter)
        {
            if (parameter is Recommendation selected)
            {
                var Page = new RecommendationPage(selected);

                Views.Wins.UserWindow.frame.NavigationService.Navigate(Page);
            }
        }

        private IEnumerable<Album> LoadAlbums()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Albums.ToList();
            }
        }

        private IEnumerable<Playlist> LoadPlaylists()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.PlaylistUsers.Where(ft => ft.UserId == LoginViewModel.CurrentUser.Id).Include(ft => ft.Playlist).Select(ft => ft.Playlist).ToList();
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

        private void OpenPlaylist(object parameter)
        {
            if (parameter is Playlist selectedPlaylist)
            {
                var playlistPage = new PlaylistPage(selectedPlaylist);

                Views.Wins.UserWindow.frame.NavigationService.Navigate(playlistPage);
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

        private void PlayPauseRecommendation(Recommendation recom)
        {
            if (player == null || recom == null)
            {
                return;
            }

            List<Song> songs = new List<Song>();

            if (player.IsPlaying && player.AlbumPlaylist == recom)
            {
                player.Pause();
                return;
            }
            else if (!player.IsPlaying && player.AlbumPlaylist == recom)
            {
                player.Play();
                return;
            }

            songs = GetSongsFromRecom(recom);

            player.AlbumPlaylist = recom;
            player.SetQueue(songs);
            player.SetSong();
            player.Play();
        }

        private void PlayPausePlaylist(Playlist playlist)
        {
            if (player == null || playlist == null)
            {
                return;
            }

            List<Song> songs = new List<Song>();

            if (player.IsPlaying && player.AlbumPlaylist == playlist)
            {
                player.Pause();
                return;
            }
            else if (!player.IsPlaying && player.AlbumPlaylist == playlist)
            {
                player.Play();
                return;
            }

            songs = GetSongsFromPlaylist(playlist);

            player.AlbumPlaylist = playlist;
            player.SetQueue(songs);
            player.SetSong();
            player.Play();
        }

        public List<Song> GetSongsFromRecom(Recommendation recom)
        {
            var result = new List<Song>();
            using (var db = new ApplicationContext())
            {
                var recommendation = db.UserRecommendations
                    .FirstOrDefault(r => r.UserId == LoginViewModel.CurrentUser.Id);

                if (recommendation?.Recommendations != null && recommendation.Recommendations.Any())
                {
                    var songs = db.Songs
                        .Where(s => recommendation.Recommendations.Contains(s.Id))
                    .ToList();

                    result = new List<Song>(songs);
                }
            }

            return result;
        }

        public List<Song> GetSongsFromPlaylist(Playlist playlist)
        {
            List<Song> songs = new List<Song>();

            using (ApplicationContext db = new ApplicationContext())
            {
                if (playlist != null)
                {
                    var playlistWithSongs = db.Playlists
                .Include(a => a.PlaylistSongs)
                .ThenInclude(als => als.Song)
                .FirstOrDefault(a => a.Id == playlist.Id);

                    if (playlistWithSongs != null)
                    {
                        foreach (PlaylistSongs s in playlistWithSongs.PlaylistSongs)
                        {
                            if (s.Song != null)
                            {
                                songs.Add(s.Song);
                            }
                        }
                    }
                }
                songs = songs.OrderBy(x => x.TrackNumber).ToList();
            }
            var sortedSongs = new List<Song>(songs);
            return sortedSongs;
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
