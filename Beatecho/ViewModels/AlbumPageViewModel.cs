using Beatecho.DAL;
using Beatecho.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Beatecho.ViewModels
{
    public class AlbumPageViewModel : INotifyPropertyChanged
    {

        public ICommand PlaySongCommand { get; }

        private Player player;
        private ObservableCollection<Song> _songs;
        private Album _album;

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
            Songs = GetSongsFromAlbum(album);
            player = PlayerViewModel.player;
            PlaySongCommand = new RelayCommand<object>(PlaySong);
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

            player.SetQueue(Songs.ToList());
            player.Index = song.TrackNumber - 1;
            player.SetSong();
            player.Play();
        }

        public ObservableCollection<Song> GetSongsFromAlbum(Album album)
        {
            List<Song> songs = new List<Song>();

            using (ApplicationContext db = new ApplicationContext())
            {
                if (album != null)
                {
                    var albumWithSongs = db.Albums
                .Include(a => a.AlbumSongs)
                .ThenInclude(als => als.Song)
                .FirstOrDefault(a => a.Id == album.Id);

                    if (albumWithSongs != null)
                    {
                        foreach (AlbumSongs s in albumWithSongs.AlbumSongs)
                        {
                            if (s.Song != null)
                            {
                                songs.Add(s.Song);
                            }
                        }
                    }
                }
                songs = songs.OrderBy(x => x.TrackNumber).ToList();
            }
            var sortedSongs = new ObservableCollection<Song>(songs);
            return sortedSongs;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
