using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Beatecho.DAL;
using Beatecho.DAL.Models;

namespace Beatecho.Wins
{
    /// <summary>
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    public partial class MainWin : Window
    {
        List<Song> songs;
        Player player;

        public MainWin()
        {
            InitializeComponent();
            player = new Player(mediaElement, CurrentSongBar, TrackSlider);
            ContentFrame.NavigationService.Navigate(new Pages.MainPage(player));
            /*lw1.ItemsSource = songs;*/
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            player.PlayNext();
        }

        private void Previous(object sender, RoutedEventArgs e)
        {
            player.PlayPrev();
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            if (player.IsPlaying)
                player.Pause();
            else
                if (player.CurrentSong != null)
                player.Play();
        }

        private void TrackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.ValueOfBarChanged();
        }

        private void TrackSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            player.DragStarted();
        }

        private void TrackSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            player.DragCompleted();
        }

        private void MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                TrackSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
            else
            {
                TrackSlider.Maximum = 100;
            }
        }

        private void MediaEnded(object sender, RoutedEventArgs e)
        {
            player.MediaEnded();
        }

        private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(TrackSlider);
            double newValue = (position.X / TrackSlider.ActualWidth) * TrackSlider.Maximum;
            TrackSlider.Value = newValue;
            player.ValueOfBarChanged();
        }

        private void Volume_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mediaElement.Volume = volumeSlider.Value;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = volumeSlider.Value;
        }

        /*private void Play(object sender, RoutedEventArgs e)
{
var editButton = sender as Button;
var selected = editButton.DataContext as Song;

try
{
mediaElement.Source = new Uri(selected.Link);
mediaElement.Play();
}
catch (Exception ex)
{
MessageBox.Show(ex.Message);
}
}

private void Pause(object sender, RoutedEventArgs e)
{
try
{
mediaElement.Pause();
}
catch (Exception ex)
{
MessageBox.Show(ex.Message);
}
}

private void Stop(object sender, RoutedEventArgs e)
{
try
{
mediaElement.Stop();
}
catch (Exception ex)
{
MessageBox.Show(ex.Message);
}
}

private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
{
MessageBox.Show(e.ErrorException.Message);
}*/
    }
}
