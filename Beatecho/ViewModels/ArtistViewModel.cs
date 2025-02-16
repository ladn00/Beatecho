using Beatecho.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using System.Reflection.Metadata;

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}