using Beatecho.DAL;
using Beatecho.DAL.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Beatecho.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                CollectionView.Refresh(); // Обновление фильтра при изменении поиска
            }
        }

        public ObservableCollection<object> Items { get; set; }
        public ICollectionView CollectionView { get; }

        public SearchViewModel()
        {
            Items = new ObservableCollection<object>();

            var albums = new ObservableCollection<Album>();
            var songs = new ObservableCollection<Song>();
            var artists = new ObservableCollection<Artist>();
            var playlists = new ObservableCollection<Playlist>();

            using (ApplicationContext db = new ApplicationContext())
            {
                albums = new ObservableCollection<Album>(db.Albums);
                songs = new ObservableCollection<Song>(db.Songs);
                artists = new ObservableCollection<Artist>(db.Artists);
                playlists = new ObservableCollection<Playlist>(db.Playlists.Where(x => x.IsPublic == true));
            }

            foreach (var album in albums) Items.Add(album);
            foreach (var song in songs) Items.Add(song);
            foreach (var artist in artists) Items.Add(artist);
            foreach (var playlist in playlists) Items.Add(playlist);

            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterItems;
        }

        private bool FilterItems(object item)
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return true;

            return item is Album album && album.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                   item is Song song && song.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                   item is Artist artist && artist.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                   item is Playlist playlist && playlist.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
