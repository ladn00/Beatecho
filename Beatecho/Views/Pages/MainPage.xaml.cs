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
using System.Diagnostics.Eventing.Reader;
using Beatecho.ViewModels;
using System.Net.NetworkInformation;

namespace Beatecho.Views.Pages
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
        }
    }
}
