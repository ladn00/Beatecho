using Beatecho.DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Beatecho.ViewModels
{
    public class PlayerViewModel : INotifyPropertyChanged
    {
        public static Player _player;
        private Song _currentSong;
        private bool _isPlaying;

        public PlayerViewModel()
        {
            PlayCommand = new RelayCommand(Play);
            PauseCommand = new RelayCommand(Pause);
            StopCommand = new RelayCommand(Stop);
            NextCommand = new RelayCommand(PlayNext);
            PreviousCommand = new RelayCommand(PlayPrevious);
        }

        public ObservableCollection<Song> Queue { get; set; } = new ObservableCollection<Song>();

        public Song CurrentSong
        {
            get => _currentSong;
            set
            {
                _currentSong = value;
                OnPropertyChanged(nameof(CurrentSong));
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }

        private void Play()
        {
            _player.Play();
            IsPlaying = true;
        }

        private void Pause()
        {
            _player.Pause();
            IsPlaying = false;
        }

        private void Stop()
        {
            _player.Stop();
            IsPlaying = false;
        }

        private void PlayNext()
        {
            _player.PlayNext();
            CurrentSong = _player.CurrentSong;
        }

        private void PlayPrevious()
        {
            _player.PlayPrev();
            CurrentSong = _player.CurrentSong;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
