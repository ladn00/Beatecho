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
using System.Windows;
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
        private ObservableCollection<Artist> _artists;
        private int _selectedArtistId;

        public int SelectedArtistId
        {
            get => _selectedArtistId;
            set
            {
                _selectedArtistId = value;
                OnPropertyChanged(nameof(SelectedArtistId));
            }
        }
        public ObservableCollection<Artist> Artists
        {
            get => _artists;
            set
            {
                _artists = value;
                OnPropertyChanged(nameof(Artists));
            }
        }

        public AlbumWindowViewModel(Album album)
        {
            player = PlayerViewModel.player;
            _currentUser = LoginViewModel.CurrentUser;
            this.Album = album;
            using (ApplicationContext db = new ApplicationContext())
            {
                Artists = new ObservableCollection<Artist>(db.Artists.ToList());
                var firstArtist = db.ArtistAlbums
                                .Where(aa => aa.AlbumId == album.Id)
                                .Select(aa => aa.ArtistId)
                                .FirstOrDefault();

                SelectedArtistId = firstArtist;
            }

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
                OnPropertyChanged(nameof(Album));
            }
        }

        public void SaveAlbum()
        {
            if (Album != null)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Album dbAlbum;

                    if (Album.Id == 0)
                    {
                        Album.Id = db.Albums.Any() ? db.Albums.Max(x => x.Id) + 1 : 1;
                        db.Albums.Add(Album);
                        db.SaveChanges();
                    }
                    else
                    {
                        dbAlbum = db.Albums.FirstOrDefault(a => a.Id == Album.Id);
                        if (dbAlbum != null)
                        {
                            dbAlbum.Title = Album.Title;
                            dbAlbum.ReleaseYear = Album.ReleaseYear;
                            dbAlbum.Photo = Album.Photo;
                            db.SaveChanges();
                        }
                    }

                    var existingLinks = db.ArtistAlbums.Where(aa => aa.AlbumId == Album.Id);
                    db.ArtistAlbums.RemoveRange(existingLinks);

                    var newArtistAlbum = new ArtistAlbums
                    {
                        AlbumId = Album.Id,
                        ArtistId = SelectedArtistId
                    };
                    db.ArtistAlbums.Add(newArtistAlbum);
                    db.SaveChanges();
                }

                MessageBox.Show("Альбом успешно сохранён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void SelectImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                Title = "Выберите изображение для альбома"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string extension = System.IO.Path.GetExtension(openFileDialog.FileName);
                string guidFileName = Guid.NewGuid().ToString() + extension;

                string ftpUrl = @"ftp://ff771a7312a3.hosting.myjino.ru/domains/ff771a7312a3.hosting.myjino.ru/Covers/" + guidFileName;
                string publicUrl = @"https://ff771a7312a3.hosting.myjino.ru/Covers/" + guidFileName;

                FTPClient ftp = new FTPClient("195.161.41.36", "j78877840", "178982az");
                bool uploadSuccess = ftp.UploadFile(openFileDialog.FileName, ftpUrl);

                if (uploadSuccess)
                {
                    Album.Photo = publicUrl;
                    OnPropertyChanged(nameof(Album.Photo));
                    MessageBox.Show("Изображение успешно загружено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при загрузке изображения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
