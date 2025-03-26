﻿using Beatecho.DAL.Models;
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
using Button = System.Windows.Controls.Button;

namespace Beatecho.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AlbumPage.xaml
    /// </summary>
    public partial class AlbumPage : Page
    {
        public AlbumPage(Album album)
        {
            InitializeComponent();
            var vm = new AlbumPageViewModel(album);
            DataContext = vm;

            Loaded += async (_, __) => await vm.InitializeAsync();
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
