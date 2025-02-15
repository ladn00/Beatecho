using Beatecho.DAL.Models;
using Beatecho.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace Beatecho.ViewModels
{
    internal class FavoriteTracksViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Song> _favoriteSongs;
        private readonly Player player;
        private readonly User user;

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
        public ICommand AddToFavoritesCommand { get; }
        public ICommand GoToArtistCommand { get; }
        public ICommand AddToPlaylistCommand { get; }

        public FavoriteTracksViewModel(User user)
        {
            player = PlayerViewModel.player;
            PlaySongCommand = new RelayCommand<object>(PlaySong);
            AddToFavoritesCommand = new RelayCommand<Song>(RemoveFromFavorites);
            LoadFavoriteSongs();
            this.user = user;
        }

        private void LoadFavoriteSongs()
        {
            using (var context = new ApplicationContext())
            {
                var songs = context.FavoriteTracks
                    .Where(ft => ft.UserId == 1)
                    .Include(ft => ft.Song)
                    .Select(ft => ft.Song)
                    .ToList();

                FavoriteSongs = new ObservableCollection<Song>(songs);
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

            player.SetQueue(FavoriteSongs.ToList());
            player.Index = FavoriteSongs.IndexOf(song);
            player.SetSong();
            player.Play();
        }

        private void RemoveFromFavorites(Song song)
        {
            using (var context = new ApplicationContext())
            {
                var favoriteTrack = context.FavoriteTracks
                    .FirstOrDefault(ft => ft.UserId == 1 && ft.SongId == song.Id);

                if (favoriteTrack != null)
                {
                    context.FavoriteTracks.Remove(favoriteTrack);
                    context.SaveChanges();
                    FavoriteSongs.Remove(song);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
