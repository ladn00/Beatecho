using Beatecho.DAL.Models;
using Beatecho.Views.Wins;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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
            _currentUser = LoginViewModel.CurrentUser;
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
                        Playlist.Id = db.Playlists.Any() ? db.Playlists.Max(x => x.Id) + 1 : 1;
                        db.Playlists.Add(Playlist);

                        PlaylistUsers playlistUsers = new PlaylistUsers
                        {
                            PlaylistId = Playlist.Id,
                            UserId = _currentUser.Id
                        };
                        db.PlaylistUsers.Add(playlistUsers);
                    }
                    else
                    {
                        var dbPlaylist = db.Playlists.FirstOrDefault(p => p.Id == Playlist.Id);
                        if (dbPlaylist != null)
                        {
                            dbPlaylist.Title = Playlist.Title;
                            dbPlaylist.Photo = Playlist.Photo;
                            dbPlaylist.IsPublic = Playlist.IsPublic;
                        }
                    }

                    db.SaveChanges();
                    MessageBox.Show("Плейлист успешно сохранён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
                string extension = System.IO.Path.GetExtension(openFileDialog.FileName);
                string guidFileName = Guid.NewGuid().ToString() + extension;

                var destinationFolder = "../../../imgs/Playlists";
                var destinationPath = System.IO.Path.Combine(destinationFolder, guidFileName);

                try
                {
                    System.IO.File.Copy(openFileDialog.FileName, destinationPath, overwrite: true);
                    Playlist.Photo = guidFileName;
                    OnPropertyChanged(nameof(Playlist.Photo));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public void AddNewPlaylist()
        {
            win = new AddOrEditNewPlaylistWindow(new Playlist() { Id = 0 });
            win.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
