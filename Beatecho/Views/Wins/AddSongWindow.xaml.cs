using Beatecho.DAL.Models;
using Beatecho.ViewModels;
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

namespace Beatecho.Views.Wins
{
    /// <summary>
    /// Логика взаимодействия для AddSongWindow.xaml
    /// </summary>
    public partial class AddSongWindow : Window
    {
        public AddSongWindow(Album album)
        {
            InitializeComponent();
            var vm = new SongWindowViewModel(album, new Song { Id = 0 });
            vm.Added += CloseWindow;
            DataContext = vm;
        }

        private void CloseWindow()
        {
            this.Close();
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

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
