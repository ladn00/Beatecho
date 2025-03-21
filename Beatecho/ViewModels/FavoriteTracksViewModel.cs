﻿using Beatecho.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;

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
        public ICommand GoToArtistCommand { get; }
        public ICommand AddToPlaylistCommand { get; }
        public ICommand RemoveFromFavoritesCommand { get; }

        public FavoriteTracksViewModel()
        {
            user = LoginViewModel.CurrentUser;
            player = PlayerViewModel.player;
            PlaySongCommand = new RelayCommand<object>(PlaySong);
            RemoveFromFavoritesCommand = new RelayCommand<Song>(RemoveFromFavorites);
            LoadFavoriteSongs();
        }

        private void LoadFavoriteSongs()
        {
            using (var context = new ApplicationContext())
            {
                var songs = context.FavoriteTracks
                    .Where(ft => ft.UserId == user.Id)
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
                    .FirstOrDefault(ft => ft.UserId == user.Id && ft.SongId == song.Id);

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
