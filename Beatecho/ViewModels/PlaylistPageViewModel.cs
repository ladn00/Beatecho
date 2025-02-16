using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Beatecho.ViewModels
{
    class PlaylistPageViewModel : INotifyPropertyChanged
    {
        public ICommand PlaySongCommand { get; }
        public ICommand AddToFavoritesCommand { get; }

        private Player player;
        private ObservableCollection<Song> _songs;
        private Playlist _playlist;
        private Dictionary<int, bool> _favoritesState;
        private User _currentUser;


        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set
            {
                _songs = value;
                OnPropertyChanged(nameof(Songs));
            }
        }

        public Playlist Playlist
        {
            get => _playlist;
            set
            {
                _playlist = value;
                OnPropertyChanged(nameof(Playlist));
            }
        }


        public string GetFavoriteIcon(int songId)
        {
            return _favoritesState.ContainsKey(songId) && _favoritesState[songId] ? "❤️" : "🤍";
        }

        public bool IsSongFavorite(int songId)
        {
            return _favoritesState.ContainsKey(songId) && _favoritesState[songId];
        }

        private void LoadFavoritesState()
        {
            if (_currentUser == null) return;

            using (ApplicationContext db = new ApplicationContext())
            {
                var favorites = db.FavoriteTracks
                    .Where(ft => ft.UserId == _currentUser.Id)
                    .ToList();

                foreach (var song in Songs)
                {
                    _favoritesState[song.Id] = favorites.Any(f => f.SongId == song.Id);
                }
            }
        }

        public PlaylistPageViewModel(Playlist playlist)
        {
            Playlist = playlist;
            Songs = GetSongsFromPlaylist(playlist);
            player = PlayerViewModel.player;

            using (ApplicationContext db = new ApplicationContext())
            {
                _currentUser = db.Users.FirstOrDefault(u => u.Id == 1);
            }
            _favoritesState = new Dictionary<int, bool>();
            PlaySongCommand = new RelayCommand<object>(PlaySong);
            AddToFavoritesCommand = new RelayCommand<Song>(AddSongToFavorites);

            LoadFavoritesState();
        }

        public void AddSongToFavorites(Song song)
        {
            if (_currentUser == null || song == null)
                return;

            using (ApplicationContext db = new ApplicationContext())
            {
                var existingFavorite = db.FavoriteTracks
                    .FirstOrDefault(ft => ft.UserId == _currentUser.Id && ft.SongId == song.Id);

                if (existingFavorite == null)
                {
                    var favoriteTrack = new FavoriteTracks
                    {
                        UserId = _currentUser.Id,
                        SongId = song.Id
                    };

                    db.FavoriteTracks.Add(favoriteTrack);
                    _favoritesState[song.Id] = true;
                }
                else
                {
                    db.FavoriteTracks.Remove(existingFavorite);
                    _favoritesState[song.Id] = false;
                }
                db.SaveChanges();
            }
            OnPropertyChanged(nameof(Songs));
        }


        private void PlaySong(object parameter)
        {
            if (parameter is not Song song)
                return;

            if (!player.IsPlaying && player.CurrentSong == parameter)
            {
                player.Play();
                return;
            }
            else if (player.IsPlaying && player.CurrentSong == parameter)
            {
                player.Pause();
                return;
            }

            player.SetQueue(Songs.ToList());
            player.Index = Songs.IndexOf(song);
            player.SetSong();
            player.Play();
        }

        public ObservableCollection<Song> GetSongsFromPlaylist(Playlist playlist)
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
            var sortedSongs = new ObservableCollection<Song>(songs);
            return sortedSongs;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
