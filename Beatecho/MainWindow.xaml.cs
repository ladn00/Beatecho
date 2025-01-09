using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Wins;
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

            MainWin win = new MainWin();
            win.Show();
            this.Close();

            /*using (ApplicationContext db = new ApplicationContext())
            {

            }*/
        }
    }
}