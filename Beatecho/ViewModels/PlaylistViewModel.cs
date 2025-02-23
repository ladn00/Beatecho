using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Wins;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Beatecho.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        public ICommand AddPlaylistCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand SaveCommand { get; }
        private Player player;

        private Playlist _playlist;
        public Playlist Playlist
        {
            get => _playlist;
            set
            {
                _playlist = value;
                OnPropertyChanged(nameof(Playlist));
            }
        }

        private User _currentUser;

        private AddOrEditNewPlaylistWindow win;

        public PlaylistViewModel(Playlist playlist)
        {
            player = PlayerViewModel.player;

            using (ApplicationContext db = new ApplicationContext())
            {
                _currentUser = db.Users.FirstOrDefault(u => u.Id == 1);
            }
            this.Playlist = playlist;
            AddPlaylistCommand = new RelayCommand(AddNewPlaylist);
            SelectImageCommand = new RelayCommand(SelectImage);
            SaveCommand = new RelayCommand(SavePlaylist);

        }

        public void SavePlaylist()
        {
            if (Playlist != null)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (Playlist.Id == 0)
                    {
                        Playlist.Id = db.Playlists.Max(x => x.Id) + 1;
                        db.Playlists.Add(Playlist);
                        PlaylistUsers playlistUsers = new PlaylistUsers() { PlaylistId = Playlist.Id, UserId = _currentUser.Id };
                        db.PlaylistUsers.Add(playlistUsers);
                    }
                    db.SaveChanges();
                }
            }
        }

        public void SelectImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                Title = "Выберите изображение для плейлиста"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Playlist.Photo = System.IO.Path.GetFileName(openFileDialog.FileName);

                var destinationPath = System.IO.Path.Combine("../../../imgs/Playlists", Playlist.Photo);
                System.IO.File.Copy(openFileDialog.FileName, destinationPath, overwrite: true);
            }

            OnPropertyChanged(nameof(Playlist.Photo));
        }

        public void AddNewPlaylist()
        {
            win = new AddOrEditNewPlaylistWindow(new Playlist() { Id = 0});
            win.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
