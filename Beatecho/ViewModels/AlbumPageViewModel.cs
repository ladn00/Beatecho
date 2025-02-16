using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Beatecho.ViewModels
{
    public class AlbumPageViewModel : INotifyPropertyChanged
    {

        public ICommand PlaySongCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand AddToFavoritesCommand { get; }

        private Player player;
        private ObservableCollection<Song> _songs;
        private Album _album;
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

        public Album Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        public string FavoriteIcon
        {
            get
            {
                if (Songs == null || !Songs.Any()) return "🤍";
                var currentSong = Songs.FirstOrDefault();
                return currentSong != null ? GetFavoriteIcon(currentSong.Id) : "🤍";
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

        public AlbumPageViewModel(Album album)
        {
            Album = album;
            Songs = GetSongsFromAlbum(album);
            player = PlayerViewModel.player;

            using (ApplicationContext db = new ApplicationContext())
            {
                _currentUser = db.Users.FirstOrDefault(u => u.Id == 1);
            }
            _favoritesState = new Dictionary<int, bool>();
            PlaySongCommand = new RelayCommand<object>(PlaySong);
            NavigateToArtistCommand = new RelayCommand<Album>(NavigateToArtist);
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
