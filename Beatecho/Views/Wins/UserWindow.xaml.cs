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

namespace Beatecho.Views.Wins
{
    /// <summary>
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        List<Song> songs;
        Player player;

        public UserWindow()
        {
            InitializeComponent();
            player = new Player(mediaElement, CurrentSongBar, TrackSlider);
            ViewModels.PlayerViewModel.player = player;
            ContentFrame.NavigationService.Navigate(new Pages.MainPage(player));
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
    }
}
