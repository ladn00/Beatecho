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
using Beatecho.DAL.Models;

namespace Beatecho.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ArtistPage.xaml
    /// </summary>
    public partial class ArtistPage : Page
    {
        public ArtistPage(Artist artist)
        {
            InitializeComponent();
        }
    }
}
