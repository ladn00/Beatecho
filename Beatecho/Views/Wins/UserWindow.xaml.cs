//***********************************************************************
//*Название программы: "Beatecho"                                       *
//*                                                                     *
//*Назначение программы: прослушивание музыки, ведение музыкальной      *
// библиотеки                                                           *
//*                                                                     *
//*Разработчик: студент группы ПР-430/б Зуев А.А.                       *
//*                                                                     *
//* версия: 1.0                                                         *
//***********************************************************************

using Beatecho.DAL.Models;
using Beatecho.ViewModels;
using Beatecho.Views.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Application = System.Windows.Application;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;

namespace Beatecho.Views.Wins
{
    /// <summary>
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        Player player;
        public static Frame frame;
        public bool IsAdmin { get; set; }
        PlayerViewModel vm;

        public UserWindow()
        {
            InitializeComponent();
            player = new Player(mediaElement, CurrentSongBar, TrackSlider);
            vm = new PlayerViewModel();
            ViewModels.PlayerViewModel.player = player;
            vm.LoadPlayer();
            frame = ContentFrame;
            DataContext = vm;
            ContentFrame.NavigationService.Navigate(new MainPage());
            IsAdmin = LoginViewModel.IsAdmin;
            SetMenuVisibility();
            UserRecommendationsService rec = new UserRecommendationsService();
            Task.Run(async () =>
            rec.UpdateRecommendationsAsync(LoginViewModel.CurrentUser.Id));

        }

        private void SetMenuVisibility()
        {
            AdminMenuItem.Visibility = IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }
            else
            {
                if (WindowState == WindowState.Maximized)
                {
                    var point = PointToScreen(e.GetPosition(this));

                    WindowState = WindowState.Normal;

                    var ratio = ActualWidth / SystemParameters.WorkArea.Width;
                    Left = point.X - (ActualWidth * (point.X / SystemParameters.WorkArea.Width));
                    Top = point.Y - (e.GetPosition(this).Y * ratio);
                }

                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GoToMainPage(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new MainPage());
        }

        private void OpenPlaylist(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var playlist = button.DataContext as Playlist;
            frame.NavigationService.Navigate(new PlaylistPage(playlist));
        }

        private async void GoToArtist(object sender, MouseButtonEventArgs e)
        {
            var artistTitle = tbArtist.Text;
            using (ApplicationContext db = new ApplicationContext())
            {
                var artistFromDb = await db.Artists.FirstOrDefaultAsync(a => a.Name == artistTitle);

                if (artistFromDb != null)
                {
                    var artistPage = new ArtistPage(artistFromDb);
                    Views.Wins.UserWindow.frame.NavigationService.Navigate(artistPage);
                }
            }
        }

        private async void GoToAlbum(object sender, MouseButtonEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var albumSongs = await db.AlbumSongs.FirstOrDefaultAsync(a => a.SongId == player.CurrentSong.Id);

                if (albumSongs != null)
                {
                    var albumPage = new AlbumPage(albumSongs.Album);
                    Views.Wins.UserWindow.frame.NavigationService.Navigate(albumPage);
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            if (SearchBox != null && PlaceholderText != null)
            {
                PlaceholderText.Visibility = string.IsNullOrWhiteSpace(SearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new AdminPage());
        }

        private bool isDarkTheme = true;

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            if (isDarkTheme)
            {
                Application.Current.Resources["MainBackground"] = new SolidColorBrush(Color.FromRgb(125, 125, 125));
                Application.Current.Resources["ScndBackground"] = new SolidColorBrush(Colors.LightGray);
                Application.Current.Resources["MainFontColor"] = new SolidColorBrush(Colors.Black);
                Application.Current.Resources["CellsBackground"] = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                Application.Current.Resources["MainBackground"] = new SolidColorBrush(Colors.Black);
                Application.Current.Resources["ScndBackground"] = new SolidColorBrush(Color.FromRgb(18, 18, 18));
                Application.Current.Resources["MainFontColor"] = new SolidColorBrush(Colors.White);
                Application.Current.Resources["CellsBackground"] = new SolidColorBrush(Color.FromRgb(68, 68, 68));
            }

            isDarkTheme = !isDarkTheme;
        }

        private void GoToVinyl_Click(object sender, RoutedEventArgs e)
        {
            if(player.CurrentSong != null)
            {
                VinylAnimation win = new(vm);
                win.Show();
            }
        }
    }
}
