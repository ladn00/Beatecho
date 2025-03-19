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
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Beatecho.ViewModels
{
    public class ArtistWindowViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private AddArtistWindow win;

        public ArtistWindowViewModel(Artist artist)
        {
            player = PlayerViewModel.player;
            _currentUser = LoginViewModel.CurrentUser;
            this.Artist = artist;
            AddArtistCommand = new RelayCommand(AddNewArtist);
            SelectImageCommand = new RelayCommand(SelectImage);
            SaveCommand = new RelayCommand(SaveArtist);
        }

        public ICommand AddArtistCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand SaveCommand { get; }
        private Player player;

        private Artist _artist;
        public Artist Artist
        {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        public void SaveArtist()
        {
            if (Artist != null)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (Artist.Id == 0)
                    {
                        Artist.Id = db.Artists.Max(x => x.Id) + 1;
                        db.Artists.Add(Artist);
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
                Artist.Photo = System.IO.Path.GetFileName(openFileDialog.FileName);

                var destinationPath = System.IO.Path.Combine("../../../imgs/Artists", Artist.Photo);
                System.IO.File.Copy(openFileDialog.FileName, destinationPath, overwrite: true);
            }

            OnPropertyChanged(nameof(Playlist.Photo));
        }

        public void AddNewArtist()
        {
            win = new AddArtistWindow(new Artist() { Id = 0 });
            win.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
