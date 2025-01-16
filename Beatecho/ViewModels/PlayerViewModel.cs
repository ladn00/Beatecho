using Beatecho.DAL.Models;
using Microsoft.Xaml.Behaviors.Core;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Beatecho.ViewModels
{
    public class PlayerViewModel : INotifyPropertyChanged
    {
        public static Player player;

        public PlayerViewModel()
        {
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
        }

        public ICommand PlayPauseCommand { get; }
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

        private void PlayPause()
        {
            if(player.IsPlaying)
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
