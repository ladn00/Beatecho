using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Microsoft.Xaml.Behaviors.Core;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;

namespace Beatecho.ViewModels
{
    public class PlayerViewModel : INotifyPropertyChanged
    {
        public static Player player;
        public PlaylistViewModel PlaylistViewModel { get; set; }
        private string _playPauseImage = "/imgs/playbut.png";
        public string PlayPauseImage
        {
            get => _playPauseImage;
            set
            {
                if (_playPauseImage != value)
                {
                    _playPauseImage = value;
                    OnPropertyChanged(nameof(PlayPauseImage));
                }
            }
        }

        public PlayerViewModel()
        {
            PlaylistViewModel = new PlaylistViewModel(new Playlist() { Id = 0 });
            PlayPauseCommand = new RelayCommand(PlayPause);
            NextCommand = new RelayCommand(PlayNext);
            PreviousCommand = new RelayCommand(PlayPrevious);
            SliderValueChangedCommand = new RelayCommand(OnSliderValueChanged);
            SliderDragStartedCommand = new RelayCommand(OnSliderDragStarted);
            SliderDragCompletedCommand = new RelayCommand(OnSliderDragCompleted);
            MediaOpenedCommand = new ActionCommand(OnMediaOpened);
            MediaEndedCommand = new ActionCommand(OnMediaEnded);
            VolumeChangedCommand = new RelayCommand<double>(OnVolumeChanged);
            VolumeMouseDownCommand = new RelayCommand<Slider>(OnVolumeMouseDown);
            TrackPreviewMouseDownCommand = new RelayCommand<Slider>(TrackMouseDown);
            OpenFavoritesCommand = new RelayCommand(OpenFavorites);
            ShuffleCommand = new RelayCommand(ShuffleQueue);
        }

        public void ShuffleQueue()
        {
            if (!player.IsShuffled)
            {
                player.ShuffleQueue();
            }
            else
            {
                player.UnshuffleQueue();
            }
        }

        public void LoadPlayer()
        {
            player.PlayPauseChanged += OnPlayPauseChanged;
        }

        private void OnPlayPauseChanged()
        {
            PlayPauseImage = player.IsPlaying ? "/imgs/pause.png" : "/imgs/playbut.png";
        }

        public ICommand PlayPauseCommand { get; }
        public ICommand ShuffleCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand SliderValueChangedCommand { get; }
        public ICommand SliderDragStartedCommand { get; }
        public ICommand SliderDragCompletedCommand { get; }
        public ICommand MediaOpenedCommand { get; }
        public ICommand MediaEndedCommand { get; }
        public ICommand VolumeChangedCommand { get; }
        public ICommand VolumeMouseDownCommand { get; }
        public ICommand TrackPreviewMouseDownCommand { get; }
        public ICommand OpenFavoritesCommand { get; }

        private void PlayPause()
        {
            if (player.IsPlaying)
            {
                player.Pause();
            }
            else
            {
                player.Play();
            }
        }

        private void PlayNext()
        {
            player.PlayNext();
        }

        private void PlayPrevious()
        {
            player.PlayPrev();
        }

        private void OnSliderValueChanged()
        {
            player.ValueOfBarChanged();
        }

        private void OnSliderDragStarted()
        {
            player.DragStarted();
        }

        private void OnSliderDragCompleted()
        {
            player.DragCompleted();
        }

        private void TrackMouseDown(Slider slider)
        {
            var mousePosition = Mouse.GetPosition(slider);
            double newValue = (mousePosition.X / slider.ActualWidth) * slider.Maximum;
            slider.Value = newValue;
            player.ValueOfBarChanged();
        }

        private void OnMediaOpened()
        {
            if (player.MediaElement.NaturalDuration.HasTimeSpan)
            {
                player.TrackSlider.Maximum = player.MediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
            else
            {
                player.TrackSlider.Maximum = 100;
            }
        }

        private void OnMediaEnded()
        {
            player.MediaEnded();
        }

        private void OnVolumeChanged(double newVolume)
        {
            player.MediaElement.Volume = newVolume / 100;
        }

        private void OnVolumeMouseDown(Slider slider)
        {
            var mousePosition = Mouse.GetPosition(slider);
            double normalizedPosition = mousePosition.X / slider.ActualWidth;
            double newValue = slider.Minimum + (slider.Maximum - slider.Minimum) * normalizedPosition;
            slider.Value = newValue;
            player.MediaElement.Volume = newValue;
        }

        private void OpenFavorites()
        {
            var favoritePage = new FavoritesPage();

            Views.Wins.UserWindow.frame.NavigationService.Navigate(favoritePage);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
