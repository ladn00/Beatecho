using Beatecho.DAL;
using Beatecho.DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Beatecho
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        public MainWindow()
        {
            InitializeComponent();

            Views.Wins.UserWindow win = new Views.Wins.UserWindow();

            
            win.Show();
            this.Close();

            /*using (ApplicationContext db = new ApplicationContext())
            {

            }*/
        }
    }
}