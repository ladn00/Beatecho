﻿using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Beatecho.Views.Wins;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;

namespace Beatecho.ViewModels
{
    public class AlbumPageViewModel : INotifyPropertyChanged
    {
        public ICommand PlaySongCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand AddToFavoritesCommand { get; }
        public ICommand AddToPlaylistCommand { get; }
        public ICommand EditAlbumCommand { get; }
        public ICommand DeleteAlbumCommand { get; }
        public ICommand AddSongsCommand { get; }

        private Player player;
        private ObservableCollection<Song> _songs;
        private Album _album;
        private User _currentUser;
        private Dictionary<int, bool> _favoritesState;
        public bool IsAdmin => LoginViewModel.IsAdmin;

        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set
            {
                _songs = value;
                OnPropertyChanged(nameof(Songs));
            }
        }

        public Album Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        public AlbumPageViewModel(Album album)
        {
            Album = album;
            Songs = GetSongsFromAlbum(album);
            player = PlayerViewModel.player;
            _favoritesState = new Dictionary<int, bool>();
            _currentUser = LoginViewModel.CurrentUser;

            PlaySongCommand = new RelayCommand<object>(PlaySong);
            NavigateToArtistCommand = new RelayCommand<Album>(NavigateToArtist);
            AddToFavoritesCommand = new RelayCommand<Song>(AddSongToFavorites);
            AddToPlaylistCommand = new RelayCommand<Song>(AddSongToPlaylist);
            EditAlbumCommand = new RelayCommand(EditAlbum);
            DeleteAlbumCommand = new RelayCommand(DeleteAlbum);
            AddSongsCommand = new RelayCommand(AddSongs);
            LoadFavoritesState();
        }

        private void AddSongToPlaylist(Song song)
        {
            if (song == null)
                return;

            var addToPlaylistWindow = new AddToPlaylistWindow(song);
            addToPlaylistWindow.ShowDialog();
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
            OnPropertyChanged(nameof(Album));
            OnPropertyChanged($"Item[{song.Id}]");
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

        public bool IsSongFavorite(int songId)
        {
            return _favoritesState.ContainsKey(songId) && _favoritesState[songId];
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
            player.Index = song.TrackNumber - 1;
            player.SetSong();
            player.Play();
        }

        private void NavigateToArtist(Album album)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var artist = db.Albums
                .Include(a => a.ArtistAlbums)
                .ThenInclude(aa => aa.Artist)
                .FirstOrDefault(a => a.Id == album.Id)
                ?.ArtistAlbums
                .FirstOrDefault()
                ?.Artist;

                if (artist != null)
                {
                    var artistPage = new ArtistPage(artist);
                    Views.Wins.UserWindow.frame.NavigationService.Navigate(artistPage);
                }
            }
        }

        public ObservableCollection<Song> GetSongsFromAlbum(Album album)
        {
            List<Song> songs = new List<Song>();

            using (ApplicationContext db = new ApplicationContext())
            {
                if (album != null)
                {
                    var albumWithSongs = db.Albums
                        .Include(a => a.AlbumSongs)
                        .ThenInclude(als => als.Song)
                        .FirstOrDefault(a => a.Id == album.Id);

                    if (albumWithSongs != null)
                    {
                        foreach (AlbumSongs s in albumWithSongs.AlbumSongs)
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

        private void EditAlbum()
        {
            var win = new AddAlbumWindow(Album);
            win.ShowDialog();
        }

        private void DeleteAlbum()
        {
            var result = System.Windows.MessageBox.Show("Удалить альбом?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new ApplicationContext())
                {
                    var album = db.Albums.Find(Album.Id);
                    if (album != null)
                    {
                        db.Albums.Remove(album);
                        db.SaveChanges();
                    }
                }
                UserWindow.frame.NavigationService.GoBack();
            }
        }

        private void AddSongs()
        {
            var win = new AddSongWindow(Album);
            win.ShowDialog();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}