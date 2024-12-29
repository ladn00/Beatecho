using Beatecho.DAL;
using Beatecho.Wins;
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
using Microsoft.EntityFrameworkCore;

namespace Beatecho.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        List<Album> albums;
        Player player;
        public MainPage(Player player)
        {
            InitializeComponent();
            this.player = player;

            using (ApplicationContext db = new ApplicationContext())
            {
                albums = db.Albums.ToList();
            }

            lw1.ItemsSource = albums;
            lw1.SelectedItem = null;
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            var editButton = sender as Button;
            var album = editButton?.DataContext as Album;
            List<Song> songs = new List<Song>();

            using (ApplicationContext db = new ApplicationContext())
            {
                if (album != null)
                {
                    db.Entry(album)
                        .Collection(a => a.AlbumSongs) // Явная загрузка AlbumSongs
                        .Load();

                    var albumSongs = album.AlbumSongs.ToList();
                    foreach (AlbumSongs s in albumSongs) 
                    {
                        songs.Add(s.Song);
                    }
                }
            }

            player.SetQueue(songs);
            player.SetSong();
            player.Play();
        }
    }
}
