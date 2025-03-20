using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Beatecho.Views.Wins;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace Beatecho.ViewModels
{
    public class LocalSongsViewModel : INotifyPropertyChanged
    {
        public ICommand OpenLocalSongsPageCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand PlaySongCommand { get; }

        private readonly Player player;
        private ObservableCollection<Song> _localSongs;

        public ObservableCollection<Song> LocalSongs
        {
            get => _localSongs;
            set
            {
                _localSongs = value;
                OnPropertyChanged(nameof(LocalSongs));
            }
        }

        public LocalSongsViewModel()
        {
            player = PlayerViewModel.player;
            PlaySongCommand = new RelayCommand<object>(PlaySong);
            OpenLocalSongsPageCommand = new RelayCommand(OpenLocalSongsPage);
            SelectFolderCommand = new RelayCommand(SelectFolder);
            Task.Run(async () => await LoadSongsFromFolderAsync());
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

            player.SetQueue(LocalSongs.ToList());
            player.Index = LocalSongs.IndexOf(song);
            player.SetSong();
            player.Play();
        }

        private void OpenLocalSongsPage()
        {
            UserWindow.frame.NavigationService.Navigate(new LocalSongsPage());
        }

        public async void SelectFolder()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    SavePathToFile(selectedPath);
                }
                else
                {
                    return;
                }
                await LoadSongsFromFolderAsync();
            }
        }

        private void SavePathToFile(string path)
        {
            string filePath = AppDomain.CurrentDomain + "..//..//..//..//..//selected_folder_path.txt";
            File.WriteAllText(filePath, path);
        }

        private async Task LoadSongsFromFolderAsync()
        {
            string filePath = AppDomain.CurrentDomain + "..//..//..//..//..//selected_folder_path.txt";

            if (!File.Exists(filePath))
                return;

            string folderPath = await File.ReadAllTextAsync(filePath);

            if (!Directory.Exists(folderPath))
                return;

            await Task.Run(() =>
            {
                var songs = new ObservableCollection<Song>();
                var files = Directory.GetFiles(folderPath, "*.mp3").Concat(Directory.GetFiles(folderPath, "*.flac")).ToArray();

                int id = 1;
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);

                    songs.Add(new Song
                    {
                        Id = 0,
                        Title = Path.GetFileNameWithoutExtension(file),
                        Duration = 0,
                        Link = file
                    });
                }

                LocalSongs = songs;
            });
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
