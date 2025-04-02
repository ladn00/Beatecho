using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Beatecho.Views.Wins;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;

namespace Beatecho.ViewModels
{
    public class RecommendationPageViewModel : INotifyPropertyChanged
    {
        public ICommand PlaySongCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand AddToFavoritesCommand { get; }
        public ICommand AddToPlaylistCommand { get; }

        private Player player;
        private ObservableCollection<Song> _songs;
        private Recommendation _recommendation;
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

        public Recommendation Recomm
        {
            get => _recommendation;
            set
            {
                _recommendation = value;
                OnPropertyChanged(nameof(Recomm));
            }
        }

        public RecommendationPageViewModel(Recommendation recommendation)
        {
            _recommendation = recommendation;
            Recomm = recommendation;
            player = PlayerViewModel.player;
            _favoritesState = new Dictionary<int, bool>();
            _currentUser = LoginViewModel.CurrentUser;

            PlaySongCommand = new RelayCommand<object>(PlaySong);
            NavigateToArtistCommand = new RelayCommand<Album>(NavigateToArtistAsync);
            AddToFavoritesCommand = new RelayCommand<Song>(AddSongToFavoritesAsync);
            AddToPlaylistCommand = new RelayCommand<Song>(AddSongToPlaylist);
            
        }

        public async Task InitializeAsync()
        {
            await LoadSongsAsync();
        }

        private void AddSongToPlaylist(Song song)
        {
            if (song == null) return;
            var addToPlaylistWindow = new AddToPlaylistWindow(song);
            addToPlaylistWindow.ShowDialog();
        }

        private async Task LoadSongsAsync()
        {
            if (_currentUser == null) return;

            using (var db = new ApplicationContext())
            {
                var recommendation = await db.UserRecommendations
                    .FirstOrDefaultAsync(r => r.UserId == _currentUser.Id);

                if (recommendation?.Recommendations != null && recommendation.Recommendations.Any())
                {
                    var songs = await db.Songs
                        .Where(s => recommendation.Recommendations.Contains(s.Id))
                        .ToListAsync();

                    Songs = new ObservableCollection<Song>(songs);
                }
                else
                {
                    Songs = new ObservableCollection<Song>();
                }

                OnPropertyChanged(nameof(Songs));
            }
        }


        public async void AddSongToFavoritesAsync(Song song)
        {
            if (_currentUser == null || song == null)
                return;

            await Task.Run(async () =>
            {
                await using var db = new ApplicationContext();
                var existingFavorite = await db.FavoriteTracks
                    .FirstOrDefaultAsync(ft => ft.UserId == _currentUser.Id && ft.SongId == song.Id);

                if (existingFavorite == null)
                {
                    var favoriteTrack = new FavoriteTracks
                    {
                        UserId = _currentUser.Id,
                        SongId = song.Id
                    };
                    await db.FavoriteTracks.AddAsync(favoriteTrack);

                    _favoritesState[song.Id] = true;
                }
                else
                {
                    db.FavoriteTracks.Remove(existingFavorite);
                    _favoritesState[song.Id] = false;
                }

                await db.SaveChangesAsync();
            });

            CollectionViewSource.GetDefaultView(Songs)?.Refresh();
        }



        private async Task LoadFavoritesStateAsync()
        {
            if (_currentUser == null) return;

            await using var db = new ApplicationContext();
            var favoriteSongIds = await db.FavoriteTracks
                .Where(ft => ft.UserId == _currentUser.Id)
                .Select(ft => ft.SongId)
                .ToListAsync();

            foreach (var song in Songs)
            {
                _favoritesState[song.Id] = favoriteSongIds.Contains(song.Id);
            }
        }

        public bool IsSongFavorite(int songId) =>
            _favoritesState.TryGetValue(songId, out bool isFav) && isFav;

        private void PlaySong(object parameter)
        {
            if (parameter is not Song song) return;

            if (!player.IsPlaying && player.CurrentSong == parameter)
            {
                player.Play();
            }
            else if (player.IsPlaying && player.CurrentSong == parameter)
            {
                player.Pause();
            }
            else
            {
                player.SetQueue(Songs.ToList());
                player.Index = song.TrackNumber - 1;
                player.SetSong();
                player.Play();
            }
        }

        private async void NavigateToArtistAsync(Album album)
        {
            await using var db = new ApplicationContext();
            var artist = await db.Albums
                .Where(a => a.Id == album.Id)
                .SelectMany(a => a.ArtistAlbums.Select(aa => aa.Artist))
                .FirstOrDefaultAsync();

            if (artist != null)
            {
                var artistPage = new ArtistPage(artist);
                Views.Wins.UserWindow.frame.NavigationService.Navigate(artistPage);
            }
        }

        public async Task LoadSongsFromAlbumAsync(Album album)
        {
            if (album == null)
            {
                Songs = new ObservableCollection<Song>();
                return;
            }

            await using var db = new ApplicationContext();
            var albumWithSongs = await db.Albums
                .Include(a => a.AlbumSongs)
                    .ThenInclude(als => als.Song)
                .FirstOrDefaultAsync(a => a.Id == album.Id);

            if (albumWithSongs != null)
            {
                var songs = albumWithSongs.AlbumSongs
                    .Select(als => als.Song)
                    .OrderBy(s => s.TrackNumber)
                    .ToList();

                Songs = new ObservableCollection<Song>(songs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}