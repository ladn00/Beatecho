using Beatecho.DAL.Models;
using Beatecho.ViewModels;
using Beatecho.Views.Wins;
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
using Application = System.Windows.Application;

namespace Beatecho.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
            var searchVM = (SearchViewModel)Application.Current.Resources["SearchViewModel"];
            searchVM.LoadData();
            DataContext = searchVM;

        }

        private void AddSongToPlaylist(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AlbumBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Album selectedAlbum)
            {
                AlbumPage albumPage = new AlbumPage(selectedAlbum);
                UserWindow.frame.NavigationService?.Navigate(albumPage);
            }
        }

        private void ArtistBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Artist selectedArtist)
            {
                ArtistPage artistPage = new ArtistPage(selectedArtist);
                NavigationService?.Navigate(artistPage);
            }
        }

        private void PlaylistBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Playlist selectedPlaylist)
            {
                PlaylistPage playlistPage = new PlaylistPage(selectedPlaylist);
                NavigationService?.Navigate(playlistPage);
            }
        }
    }
}
