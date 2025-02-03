﻿using System;
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
        Player player;
        public static Frame frame;

        public UserWindow()
        {
            InitializeComponent();
            player = new Player(mediaElement, CurrentSongBar, TrackSlider);
            ViewModels.PlayerViewModel.player = player;
            frame = ContentFrame;
            ContentFrame.NavigationService.Navigate(new Pages.MainPage(player));
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
                    // Получаем позицию курсора относительно окна
                    var point = PointToScreen(e.GetPosition(this));
                    
                    // Восстанавливаем нормальное состояние окна
                    WindowState = WindowState.Normal;

                    // Вычисляем новую позицию окна так, чтобы курсор оказался в точке захвата
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
    }
}
