using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Beatecho.Views.Wins;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;

namespace Beatecho.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        public ICommand OpenSearchPageCommand { get; }
        public ICommand PlaySongCommand { get; }

        private Player player;
        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                    FilterCollection();
                }
            }
        }

        public ObservableCollection<Song> Songs { get; set; } = new();
        public ObservableCollection<Album> Albums { get; set; } = new();
        public ObservableCollection<Artist> Artists { get; set; } = new();
        public ObservableCollection<Playlist> Playlists { get; set; } = new();

        public ICollectionView FilteredSongs { get; }
        public ICollectionView FilteredAlbums { get; }
        public ICollectionView FilteredArtists { get; }
        public ICollectionView FilteredPlaylists { get; }

        public SearchViewModel()
        {
            PlaySongCommand = new RelayCommand<object>(PlaySong);
            OpenSearchPageCommand = new RelayCommand(OpenSearchPage);

            // Инициализация коллекций (пока пустых)
            Songs = new ObservableCollection<Song>();
            Albums = new ObservableCollection<Album>();
            Artists = new ObservableCollection<Artist>();
            Playlists = new ObservableCollection<Playlist>();

            // Инициализация CollectionView
            FilteredSongs = CollectionViewSource.GetDefaultView(Songs);
            FilteredAlbums = CollectionViewSource.GetDefaultView(Albums);
            FilteredArtists = CollectionViewSource.GetDefaultView(Artists);
            FilteredPlaylists = CollectionViewSource.GetDefaultView(Playlists);

            // Асинхронно загружаем данные
            LoadData();
        }

        private void PlaySong(object parameter)
        {
            if(player == null)
                player = PlayerViewModel.player;

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

            player.SetQueue(new List<Song>() { song } );
            player.Index = 0;
            player.SetSong();
            player.Play();
        }

        public async void LoadData()
        {
            await Task.Run(() =>
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var songs = db.Songs.ToList();
                    var albums = db.Albums.ToList();
                    var artists = db.Artists.ToList();
                    var playlists = db.Playlists.Where(x => x.IsPublic == true).ToList();

                    // Обновление коллекций на UI-потоке
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Songs.Clear();
                        Albums.Clear();
                        Artists.Clear();
                        Playlists.Clear();

                        foreach (var song in songs) Songs.Add(song);
                        foreach (var album in albums) Albums.Add(album);
                        foreach (var artist in artists) Artists.Add(artist);
                        foreach (var playlist in playlists) Playlists.Add(playlist);
                    });
                }
            });
        }


        private void FilterCollection()
        {
            // Устанавливаем новый фильтр
            FilteredSongs.Filter = FilterSongs;
            FilteredAlbums.Filter = FilterAlbums;
            FilteredArtists.Filter = FilterArtists;
            FilteredPlaylists.Filter = FilterPlaylists;

            // Принудительно обновляем отображение
            FilteredSongs.Refresh();
            FilteredAlbums.Refresh();
            FilteredArtists.Refresh();
            FilteredPlaylists.Refresh();
        }


        private bool FilterSongs(object item)
        {
            return item is Song song &&
                   (string.IsNullOrWhiteSpace(SearchQuery) ||
                   song.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool FilterAlbums(object item)
        {
            return item is Album album &&
                   (string.IsNullOrWhiteSpace(SearchQuery) ||
                   album.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool FilterArtists(object item)
        {
            return item is Artist artist &&
                   (string.IsNullOrWhiteSpace(SearchQuery) ||
                   artist.Name.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool FilterPlaylists(object item)
        {
            return item is Playlist playlist &&
                   (string.IsNullOrWhiteSpace(SearchQuery) ||
                   playlist.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public void OpenSearchPage()
        {
            UserWindow.frame.NavigationService.Navigate(new SearchPage());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
