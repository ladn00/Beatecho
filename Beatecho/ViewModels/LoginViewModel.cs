using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Beatecho.Views.Wins;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;

namespace Beatecho.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event Action LoginSucceeded;
        public static User CurrentUser { get; set; }
        public static bool IsAdmin {  get; set; }

        private readonly ApplicationContext _db;
        private string _username;
        private string _password;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand CreateAccountCommand { get; }
        public ICommand ForgotPasswordCommand { get; }


        public LoginViewModel()
        {
            _db = new ApplicationContext();
            // CurrentUser = _db.Users.FirstOrDefault(u => u.Id == 1);
            
            LoginCommand = new RelayCommand<object>(ExecuteLogin);
            CreateAccountCommand = new RelayCommand<object>(ExecuteCreateAccount);
            ForgotPasswordCommand = new RelayCommand<object>(ExecuteForgotPassword);
        }

        private void ExecuteLogin(object obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Введите имя пользователя и пароль");
                    return;
                }

                var user = _db.Users.FirstOrDefault(x => x.UserName == Username && x.Password == Password);

                if (user == null)
                {
                    MessageBox.Show("Неверные данные");
                    return;
                }

                CurrentUser = user;
                IsAdmin = CurrentUser.UserTypeId == 1;

                var win = new Views.Wins.UserWindow();
                win.Show();
                LoginSucceeded?.Invoke();

            }
            catch (Exception ex) 
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void ExecuteCreateAccount(object obj)
        {
            MessageBox.Show("Redirect to Create Account page.");
        }

        private void ExecuteForgotPassword(object obj)
        {
            MessageBox.Show("Redirect to Forgot Password flow.");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
