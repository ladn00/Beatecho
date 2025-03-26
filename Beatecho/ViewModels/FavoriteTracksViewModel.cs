using Beatecho.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;

namespace Beatecho.ViewModels
{
    internal class FavoriteTracksViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Song> _favoriteSongs = new();
        private readonly Player _player;
        private readonly User _user;

        public ObservableCollection<Song> FavoriteSongs
        {
            get => _favoriteSongs;
            set
            {
                _favoriteSongs = value;
                OnPropertyChanged(nameof(FavoriteSongs));
            }
        }

        public ICommand PlaySongCommand { get; }
        public ICommand RemoveFromFavoritesCommand { get; }

        public FavoriteTracksViewModel()
        {
            _user = LoginViewModel.CurrentUser;
            _player = PlayerViewModel.player;

            PlaySongCommand = new RelayCommand<object>(PlaySong);
            RemoveFromFavoritesCommand = new RelayCommand<Song>(async song => await RemoveFromFavoritesAsync(song));

            _ = LoadFavoriteSongsAsync();
        }

        private async Task LoadFavoriteSongsAsync()
        {
            using var context = new ApplicationContext();

            var songs = await context.FavoriteTracks
                .Where(ft => ft.UserId == _user.Id)
                .Include(ft => ft.Song)
                .Select(ft => ft.Song)
                .ToListAsync();

            FavoriteSongs = new ObservableCollection<Song>(songs);
        }

        private void PlaySong(object parameter)
        {
            if (parameter is not Song song)
                return;

            if (_player.CurrentSong == song)
            {
                if (_player.IsPlaying)
                    _player.Pause();
                else
                    _player.Play();

                return;
            }

            _player.SetQueue(FavoriteSongs.ToList());
            _player.Index = FavoriteSongs.IndexOf(song);
            _player.SetSong();
            _player.Play();
        }

        private async Task RemoveFromFavoritesAsync(Song song)
        {
            if (song == null)
                return;

            using var context = new ApplicationContext();

            var favoriteTrack = await context.FavoriteTracks
                .FirstOrDefaultAsync(ft => ft.UserId == _user.Id && ft.SongId == song.Id);

            if (favoriteTrack != null)
            {
                context.FavoriteTracks.Remove(favoriteTrack);
                await context.SaveChangesAsync();

                FavoriteSongs.Remove(song);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
