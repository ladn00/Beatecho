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
    public class SongWindowViewModel : INotifyPropertyChanged
    {
        private Song _song;
        public Song Song
        {
            get => _song;
            set
            {
                _song = value;
                OnPropertyChanged(nameof(Song));
            }
        }
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

        public SongWindowViewModel(Album album, Song song)
        {
            this.Album = album;
            this.Song = song;

            SelectSongCommand = new RelayCommand(SelectSong);
            SaveCommand = new RelayCommand(SaveSong);
        }

        public ICommand AddArtistCommand { get; }
        public ICommand SelectSongCommand { get; }
        public ICommand SaveCommand { get; }

        

        public void SaveSong()
        {
            if (Song != null)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (Song.Id == 0)
                    {
                        Song.Id = db.Songs.Max(x => x.Id) + 1;
                        db.Songs.Add(Song);
                        db.SaveChanges();
                    }

                    var existingLinks = db.AlbumSongs.Where(aa => aa.SongId == Song.Id);
                    db.AlbumSongs.RemoveRange(existingLinks);

                    int nextTrackNum = db.AlbumSongs
                        .Where(x => x.AlbumId == Album.Id)
                        .Select(x => x.TrackNum)
                        .AsEnumerable()
                        .DefaultIfEmpty(0)
                        .Max() + 1;

                    db.AlbumSongs.Add(new AlbumSongs { AlbumId = Album.Id, SongId = Song.Id, TrackNum = nextTrackNum });
                    db.SaveChanges();
                }
            }
        }

        public void SelectSong()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio files (*.mp3;*.flac)|*.mp3;*.flac|All files (*.*)|*.*",
                Title = "Выберите аудиофайл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string extension = System.IO.Path.GetExtension(openFileDialog.FileName);
                string guidFileName = Guid.NewGuid().ToString() + extension;

                string ftpUrl = @"ftp://ff771a7312a3.hosting.myjino.ru/domains/ff771a7312a3.hosting.myjino.ru/" + guidFileName;
                string publicUrl = @"https://ff771a7312a3.hosting.myjino.ru/" + guidFileName;

                Song.Link = publicUrl;

                FTPClient ftp = new FTPClient("195.161.41.36", "j78877840", "178982az");

                var result = ftp.UploadFile(openFileDialog.FileName, ftpUrl);
                System.Windows.Forms.MessageBox.Show(result.ToString(), "Результат загрузки");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
