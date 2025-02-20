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
using System.Windows.Shapes;
using Beatecho.ViewModels;

namespace Beatecho.Views.Wins
{
    /// <summary>
    /// Логика взаимодействия для AddOrEditNewPlaylistWindow.xaml
    /// </summary>
    public partial class AddOrEditNewPlaylistWindow : Window
    {
        public AddOrEditNewPlaylistWindow()
        {
            InitializeComponent();
            DataContext = new PlaylistViewModel(new Playlist() { Id = 0});
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
    }
}
