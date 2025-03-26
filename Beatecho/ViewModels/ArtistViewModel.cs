using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;

namespace Beatecho.ViewModels
{
    public class ArtistViewModel : INotifyPropertyChanged
    {
        private Artist _artist;
        private ObservableCollection<Song> _popularTracks;
        private ObservableCollection<Album> _albums;
        private bool _isExpandedTracks = false;
        private readonly Player _player;
        private readonly User _currentUser;
        private Dictionary<int, bool> _favoritesState;

        public ArtistViewModel(Artist artist)
        {
            _artist = artist;
            _player = PlayerViewModel.player;
            _currentUser = LoginViewModel.CurrentUser;
            _favoritesState = new Dictionary<int, bool>();

            PlaySongCommand = new RelayCommand<object>(PlaySong);
            AddToFavoritesCommand = new RelayCommand<Song>(async (song) => await AddSongToFavoritesAsync(song));
            ShowAllTracksCommand = new RelayCommand(ExecuteShowAllTracks);
            OpenAlbumCommand = new RelayCommand<Album>(OpenAlbum);

            _ = LoadArtistData();
        }

        public Dictionary<int, bool> FavoritesState
        {
            get => _favoritesState;
            set
            {
                _favoritesState = value;
                OnPropertyChanged(nameof(FavoritesState));
            }
        }

        public Artist Artist
        {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
                OnPropertyChanged(nameof(Name));
                _ = LoadArtistData();
            }
        }

        public string Name => Artist?.Name;
        public string Photo => Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName, "imgs", "Artists", Artist.Photo);
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

        public ICommand ShowAllTracksCommand { get; set; }
        public ICommand OpenAlbumCommand { get; set; }
        public ICommand PlaySongCommand { get; set; }
        public ICommand AddToFavoritesCommand { get; set; }

        private List<Song> _allTracks;

        private async Task LoadArtistData()
        {
            using (var context = new ApplicationContext())
            {
                var albumIds = await context.ArtistAlbums
                    .Where(aa => aa.ArtistId == Artist.Id)
                    .Select(aa => aa.AlbumId)
                    .ToListAsync();

                var albums = await context.Albums
                    .Where(a => albumIds.Contains(a.Id))
                    .ToListAsync();

                Albums = new ObservableCollection<Album>(albums);

                var tracks = await context.AlbumSongs
                    .Where(als => albumIds.Contains(als.AlbumId))
                    .Include(als => als.Song)
                    .ToListAsync();

                _allTracks = tracks
                    .Select(als => als.Song)
                    .Distinct()
                    .OrderBy(s => s.TrackNumber)
                    .ToList();

                PopularTracks = new ObservableCollection<Song>(_allTracks.Take(_isExpandedTracks ? 10 : 5));

                await LoadFavoritesStateAsync(); // загружаем избранные треки после песен
            }
        }

        private async Task LoadFavoritesStateAsync()
        {
            if (_currentUser == null || PopularTracks == null) return;

            await using var db = new ApplicationContext();
            var favoriteSongIds = await db.FavoriteTracks
                .Where(ft => ft.UserId == _currentUser.Id)
                .Select(ft => ft.SongId)
                .ToListAsync();

            foreach (var song in PopularTracks)
            {
                _favoritesState[song.Id] = favoriteSongIds.Contains(song.Id);
            }

            OnPropertyChanged(nameof(FavoritesState));
        }

        private async Task AddSongToFavoritesAsync(Song song)
        {
            if (_currentUser == null || song == null) return;

            await using var db = new ApplicationContext();
            var existingFavorite = await db.FavoriteTracks
                .FirstOrDefaultAsync(ft => ft.UserId == _currentUser.Id && ft.SongId == song.Id);

            if (existingFavorite == null)
            {
                db.FavoriteTracks.Add(new FavoriteTracks { UserId = _currentUser.Id, SongId = song.Id });
                _favoritesState[song.Id] = true;
            }
            else
            {
                db.FavoriteTracks.Remove(existingFavorite);
                _favoritesState[song.Id] = false;
            }

            await db.SaveChangesAsync();
            OnPropertyChanged(nameof(FavoritesState)); // Обновляем состояние избранного
        }

        public bool IsSongFavorite(int songId)
        {
            return _favoritesState.TryGetValue(songId, out bool isFav) && isFav;
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

            PopularTracks = new ObservableCollection<Song>(_allTracks.Take(_isExpandedTracks ? 10 : 5));
        }

        private void PlaySong(object parameter)
        {
            if (parameter is not Song song) return;

            if (!_player.IsPlaying && _player.CurrentSong == parameter)
            {
                _player.Play();
                return;
            }
            else if (_player.IsPlaying && _player.CurrentSong == parameter)
            {
                _player.Pause();
                return;
            }

            _player.SetQueue(PopularTracks.ToList());
            _player.Index = PopularTracks.IndexOf(song);
            _player.SetSong();
            _player.Play();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}