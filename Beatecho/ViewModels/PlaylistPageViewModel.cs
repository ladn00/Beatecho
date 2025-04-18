﻿using Beatecho.DAL.Models;
using Beatecho.Views.Wins;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;

namespace Beatecho.ViewModels
{
    public class PlaylistPageViewModel : INotifyPropertyChanged
    {
        public ICommand PlaySongCommand { get; }
        public ICommand AddOrRemoveFromFavoritesCommand { get; }
        public ICommand EditPlaylistCommand { get; }
        public ICommand DeletePlaylistCommand { get; }
        public ICommand RemoveFromPlaylistCommand { get; }

        private Player player;
        private ObservableCollection<Song> _songs;
        private Playlist _playlist;
        private Dictionary<int, bool> _favoritesState;
        private User _currentUser;
        public bool IsPlaylistOwner
        {
            get
            {
                if (_currentUser == null || Playlist == null)
                    return false;

                using (var db = new ApplicationContext())
                {
                    var owner = db.PlaylistUsers
                        .Where(pu => pu.PlaylistId == Playlist.Id)
                        .OrderBy(pu => pu.UserId) // Если владелец — это первый добавленный пользователь
                        .Select(pu => pu.UserId)
                        .FirstOrDefault();

                    return owner == _currentUser.Id;
                }
            }
        }

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
            _currentUser = LoginViewModel.CurrentUser;

            _favoritesState = new Dictionary<int, bool>();
            PlaySongCommand = new RelayCommand<object>(PlaySong);
            AddOrRemoveFromFavoritesCommand = new RelayCommand<Song>(AddSongToFavorites);
            EditPlaylistCommand = new RelayCommand<Playlist>(EditPlaylist);
            RemoveFromPlaylistCommand = new RelayCommand<Song>(RemoveFromPlaylist);
            DeletePlaylistCommand = new RelayCommand(DeletePlaylist);
            LoadFavoritesState();

            OnPropertyChanged(nameof(IsPlaylistOwner));
        }

        private async void DeletePlaylist()
        {
            var result = MessageBox.Show("Удалить плейлист?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await using var db = new ApplicationContext();
                var playlist = await db.Playlists.FindAsync(Playlist.Id);
                var playlistUsers = await db.PlaylistUsers.Where(x => x.PlaylistId == Playlist.Id).ToListAsync();
                if(playlistUsers != null)
                {
                    foreach(var el in playlistUsers)
                    {
                        db.PlaylistUsers.Remove(el);
                    }
                    await db.SaveChangesAsync();
                }
                if (playlist != null)
                {
                    db.Playlists.Remove(playlist);
                    await db.SaveChangesAsync();
                }
                UserWindow.frame.NavigationService.GoBack();
            }
        }

        private void RemoveFromPlaylist(Song song)
        {
            if (song == null || Playlist == null)
                return;

            using (ApplicationContext db = new ApplicationContext())
            {
                var playlistWithSongs = db.Playlists
                    .Include(p => p.PlaylistSongs)
                    .FirstOrDefault(p => p.Id == Playlist.Id);

                if (playlistWithSongs != null)
                {
                    var songToRemove = playlistWithSongs.PlaylistSongs
                        .FirstOrDefault(ps => ps.SongId == song.Id);

                    if (songToRemove != null)
                    {
                        db.PlaylistSongs.Remove(songToRemove);
                        db.SaveChanges();
                    }
                }
            }

            Songs.Remove(song);
            OnPropertyChanged(nameof(Songs));
        }

        public void EditPlaylist(Playlist playlist)
        {
            var win = new AddOrEditNewPlaylistWindow(playlist);
            win.ShowDialog();
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
