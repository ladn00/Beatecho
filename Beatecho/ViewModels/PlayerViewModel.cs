using Beatecho.DAL.Models;
using System.ComponentModel;
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
        }

        public ICommand PlayPauseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand SliderValueChangedCommand { get; }
        public ICommand SliderDragStartedCommand { get; }
        public ICommand SliderDragCompletedCommand { get; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
