using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Beatecho.Views.Wins;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;

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
        public ICommand DownloadAlbumCommand { get; }

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
            player = PlayerViewModel.player;
            _favoritesState = new Dictionary<int, bool>();
            _currentUser = LoginViewModel.CurrentUser;

            PlaySongCommand = new RelayCommand<object>(PlaySong);
            NavigateToArtistCommand = new RelayCommand<Album>(NavigateToArtistAsync);
            AddToFavoritesCommand = new RelayCommand<Song>(AddSongToFavoritesAsync);
            AddToPlaylistCommand = new RelayCommand<Song>(AddSongToPlaylist);
            EditAlbumCommand = new RelayCommand(EditAlbum);
            DeleteAlbumCommand = new RelayCommand(DeleteAlbumAsync);
            AddSongsCommand = new RelayCommand(AddSongs);
            DownloadAlbumCommand = new RelayCommand(DownloadAlbumAsync);
        }

        private void DownloadAlbumAsync()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK || string.IsNullOrWhiteSpace(dialog.SelectedPath))
                return;

            string savePath = dialog.SelectedPath;

            var ftpClient = new FTPClient("195.161.41.36", "j78877840", "178982az");

            foreach (var song in Songs)
            {
                try
                {
                    var extension = Path.GetExtension(song.Link);
                    if (string.IsNullOrWhiteSpace(extension))
                    {
                        MessageBox.Show($"Не удалось определить расширение для {song.Title}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    var fileName = $"{song.Title}{extension}";
                    var localPath = Path.Combine(savePath, fileName);
                    bool success = ftpClient.DownloadFile(song.Link, localPath);

                    if (!success)
                    {
                        MessageBox.Show($"Ошибка при загрузке: {song.Title}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            MessageBox.Show("Загрузка завершена!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        public async Task InitializeAsync()
        {
            await LoadSongsFromAlbumWithFavoritesAsync(Album);
        }

        private void AddSongToPlaylist(Song song)
        {
            if (song == null) return;
            var addToPlaylistWindow = new AddToPlaylistWindow(song);
            addToPlaylistWindow.ShowDialog();
        }
        private async Task LoadSongsFromAlbumWithFavoritesAsync(Album album)
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

            if (albumWithSongs == null)
            {
                Songs = new ObservableCollection<Song>();
                return;
            }

            var songs = albumWithSongs.AlbumSongs
                .Select(als => als.Song)
                .OrderBy(s => s.TrackNumber)
                .ToList();

            if (_currentUser != null)
            {
                var favoriteSongIds = await db.FavoriteTracks
                    .Where(ft => ft.UserId == _currentUser.Id)
                    .Select(ft => ft.SongId)
                    .ToListAsync();

                foreach (var song in songs)
                {
                    _favoritesState[song.Id] = favoriteSongIds.Contains(song.Id);
                }
            }

            Songs = new ObservableCollection<Song>(songs);

            OnPropertyChanged(nameof(Songs));
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

        private void EditAlbum()
        {
            var win = new AddAlbumWindow(Album);
            win.ShowDialog();
        }

        private async void DeleteAlbumAsync()
        {
            var result = MessageBox.Show("Удалить альбом?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await using var db = new ApplicationContext();
                var album = await db.Albums.FindAsync(Album.Id);
                if (album != null)
                {
                    db.Albums.Remove(album);
                    await db.SaveChangesAsync();
                }
                UserWindow.frame.NavigationService.GoBack();
            }
        }

        private async void AddSongs()
        {
            var win = new AddSongWindow(Album);
            bool? result = win.ShowDialog();
            
            await LoadSongsFromAlbumWithFavoritesAsync(Album);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}