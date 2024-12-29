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

        public MainWin()
        {
            InitializeComponent();

            Player player = new Player(mediaElement);
            ContentFrame.NavigationService.Navigate(new Pages.MainPage(player));
            /*lw1.ItemsSource = songs;*/
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
