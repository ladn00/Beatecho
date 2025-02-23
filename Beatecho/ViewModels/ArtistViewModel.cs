using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Beatecho.ViewModels
{
    public class ArtistViewModel : INotifyPropertyChanged
    {
        private Artist _artist;
        private ObservableCollection<Song> _popularTracks;
        private ObservableCollection<Album> _albums;
        private bool _isExpandedTracks = false;
        private ICommand _showAllTracksCommand;
        private ICommand _openAlbumCommand;
        private Player player;
        private User _currentUser;
        private Dictionary<int, bool> _favoritesState;

        public Artist Artist
        {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
                OnPropertyChanged(nameof(Name));
                LoadArtistData();
            }
        }

        public ArtistViewModel(Artist artist)
        {
            Artist = artist;
            player = PlayerViewModel.player;
            PlaySongCommand = new RelayCommand<object>(PlaySong);
            ExecuteShowAllTracks();

            using (ApplicationContext db = new ApplicationContext())
            {
                _currentUser = db.Users.FirstOrDefault(u => u.Id == 1);
            }

            _favoritesState = new Dictionary<int, bool>();
            LoadFavoritesState();
            AddToFavoritesCommand = new RelayCommand<Song>(AddSongToFavorites);
        }

        public string Name => Artist?.Name;
        public string Photo => $"pack://application:,,,/imgs/Artists/{Artist?.Photo}";
        public string MonthlyListeners => "1,234,567 ежемесячных слушателей"; // Заглушка

        public ObservableCollection<Song> PopularTracks
        {
            get => _popularTracks;
            set
            {
                _popularTracks = value;
                OnPropertyChanged(nameof(PopularTracks));
            }
        }

        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set
            {
                _albums = value;
                OnPropertyChanged(nameof(Albums));
            }
        }

        public ICommand ShowAllTracksCommand => _showAllTracksCommand ??= new RelayCommand(ExecuteShowAllTracks);
        public ICommand OpenAlbumCommand => _openAlbumCommand ??= new RelayCommand<Album>(OpenAlbum);
        public ICommand PlaySongCommand { get; }
        public ICommand AddToFavoritesCommand { get; }

        private void LoadArtistData()
        {
            using (var context = new ApplicationContext())
            {
                // Загрузка альбомов артиста
                var albums = context.Artists
                    .Include(a => a.ArtistAlbums)
                    .ThenInclude(aa => aa.Album)
                    .FirstOrDefault(a => a.Id == Artist.Id)
                    ?.ArtistAlbums
                    .Select(aa => aa.Album)
                    .ToList();

                Albums = new ObservableCollection<Album>(albums ?? new List<Album>());

                // Загрузка треков артиста (первые 5 или 10)
                var tracks = context.Albums
                    .Where(a => a.ArtistAlbums.Any(aa => aa.ArtistId == Artist.Id))
                    .SelectMany(a => a.AlbumSongs)
                    .Select(ass => ass.Song)
                    .Take(_isExpandedTracks ? 10 : 5)
                    .ToList();

                PopularTracks = new ObservableCollection<Song>(tracks);
            }
        }

        private void OpenAlbum(Album album)
        {
            if (album != null)
            {
                var albumPage = new AlbumPage(album);
                Views.Wins.UserWindow.frame.NavigationService.Navigate(albumPage);
            }
        }

        private void ExecuteShowAllTracks()
        {
            _isExpandedTracks = !_isExpandedTracks;
            LoadArtistData();
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
            OnPropertyChanged(nameof(PopularTracks));
            OnPropertyChanged(nameof(IsSongFavorite));
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

            player.SetQueue(PopularTracks.ToList());
            player.Index = PopularTracks.IndexOf(song);
            player.SetSong();
            player.Play();
        }

        private void LoadFavoritesState()
        {
            if (_currentUser == null) return;

            using (ApplicationContext db = new ApplicationContext())
            {
                var favorites = db.FavoriteTracks
                    .Where(ft => ft.UserId == _currentUser.Id)
                    .ToList();

                foreach (var song in PopularTracks)
                {
                    _favoritesState[song.Id] = favorites.Any(f => f.SongId == song.Id);
                }
            }
        }

        public bool IsSongFavorite(int songId)
        {
            return _favoritesState.ContainsKey(songId) && _favoritesState[songId];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}