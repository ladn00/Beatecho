using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.ViewModels;
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
        LoginViewModel vm;
        public MainWindow()
        {
            InitializeComponent();

            vm = new LoginViewModel();
            vm.LoginSucceeded += CloseWindow;
            DataContext = vm;
            /*UserRecommendationsService rec = new UserRecommendationsService();
            Task.Run(async () => 
            rec.UpdateRecommendationsAsync(LoginViewModel.CurrentUser.Id));*/
        }

        private void CloseWindow()
        {
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            vm.Password = ((PasswordBox)sender).Password;
        }

        private void CreateAccount_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is LoginViewModel vm && vm.CreateAccountCommand.CanExecute(null))
            {
                vm.CreateAccountCommand.Execute(null);
            }
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is LoginViewModel vm && vm.ForgotPasswordCommand.CanExecute(null))
            {
                vm.ForgotPasswordCommand.Execute(null);
            }
        }
    }
}