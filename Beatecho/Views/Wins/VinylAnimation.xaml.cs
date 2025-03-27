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
using System.Windows.Media.Animation;
using Point = System.Windows.Point;
using Beatecho.ViewModels;

namespace Beatecho.Views.Wins
{
    /// <summary>
    /// Логика взаимодействия для VinylAnimation.xaml
    /// </summary>
    public partial class VinylAnimation : Window
    {
        public VinylAnimation(PlayerViewModel vm)
        {
            InitializeComponent();
            RotateVinyl();
          // Photo.ImageSource = new BitmapImage(new Uri(link, UriKind.Absolute));
            DataContext = vm;
        }

        private void RotateVinyl()
        {
            var rotateTransform = new RotateTransform();
            Vinyl.RenderTransform = rotateTransform;
            Vinyl.RenderTransformOrigin = new Point(0.5, 0.5);

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(3),
                RepeatBehavior = RepeatBehavior.Forever
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }
    }
}
