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
using static System.Net.WebRequestMethods;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Beatecho.ViewModels
{
    public class AlbumWindowViewModel : INotifyPropertyChanged
    {

        private User _currentUser;
        private AddAlbumWindow win;

        public AlbumWindowViewModel(Album album)
        {
            player = PlayerViewModel.player;
            _currentUser = LoginViewModel.CurrentUser;
            this.Album = album;
            AddArtistCommand = new RelayCommand(AddNewAlbum);
            SelectImageCommand = new RelayCommand(SelectImage);
            SaveCommand = new RelayCommand(SaveAlbum);
        }

        public ICommand AddArtistCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand SaveCommand { get; }
        private Player player;

        private Album _album;
        public Album Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        public void SaveAlbum()
        {
            if (Album != null)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (Album.Id == 0)
                    {
                        Album.Id = db.Albums.Max(x => x.Id) + 1;
                        db.Albums.Add(Album);
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
                string fileNameAndExt = System.IO.Path.GetFileName(openFileDialog.FileName);
                
            }

            OnPropertyChanged(nameof(Playlist.Photo));
        }

        public void AddNewAlbum()
        {
            win = new AddAlbumWindow(new Album() { Id = 0 });
            win.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
