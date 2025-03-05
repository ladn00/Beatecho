using Beatecho.DAL.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Beatecho.ViewModels;
using Beatecho.Views.Wins;
using Button = System.Windows.Controls.Button;

namespace Beatecho.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для PlaylistPage.xaml
    /// </summary>
    public partial class PlaylistPage : Page
    {
        public PlaylistPage(Playlist playlist)
        {
            InitializeComponent();
            DataContext = new PlaylistPageViewModel(playlist);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
            e.Handled = true;
        }

        private void AddSongToPlaylist(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var song = button.DataContext as Song;

            if (song == null)
                return;

            var addToPlaylistWindow = new AddToPlaylistWindow(song);
            addToPlaylistWindow.ShowDialog();
        }
    }
}
