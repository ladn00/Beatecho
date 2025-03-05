using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;

namespace Beatecho.Views.Wins
{
    public partial class AddToPlaylistWindow : Window
    {
        public Song SelectedSong { get; set; }
        public List<Playlist> Playlists { get; set; }

        public ICommand SelectPlaylistCommand { get; }

        public AddToPlaylistWindow(Song song)
        {
            InitializeComponent();
            SelectedSong = song;
            DataContext = this;

            LoadPlaylists();

            SelectPlaylistCommand = new RelayCommand<Playlist>(SelectPlaylist);
        }

        private void LoadPlaylists()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Playlists = db.Playlists.ToList();
            }
        }

        private void SelectPlaylist(Playlist playlist)
        {
            if (playlist == null || SelectedSong == null)
                return;

            using (ApplicationContext db = new ApplicationContext())
            {
                var playlistSong = new PlaylistSongs
                {
                    PlaylistId = playlist.Id,
                    SongId = SelectedSong.Id
                };

                if(db.PlaylistSongs.Contains(playlistSong))
                {
                    MessageBox.Show("Эта песня уже есть в выбранном плейлисте");
                    return;
                }    

                db.PlaylistSongs.Add(playlistSong);
                db.SaveChanges();
            }

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}