using Beatecho.DAL.Models;
using Beatecho.Views.Wins;
using System.Windows;
using System.Windows.Controls;

namespace Beatecho.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void OpenAddArtist_Click(object sender, RoutedEventArgs e)
        {
            AddArtistWindow win = new AddArtistWindow(new Artist { Id = 0 });
            win.ShowDialog();
        }

        private void OpenAddAlbum_Click(object sender, RoutedEventArgs e)
        {
            AddAlbumWindow win = new AddAlbumWindow(new Album { Id = 0 });
            win.ShowDialog();
        }
    }
}
