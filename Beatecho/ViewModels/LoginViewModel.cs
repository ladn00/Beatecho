using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Beatecho.Views.Wins;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using MessageBox = System.Windows.MessageBox;

namespace Beatecho.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public static User CurrentUser { get; set; }
        private readonly ApplicationContext _db;

        public LoginViewModel()
        {
            _db = new ApplicationContext();
            CurrentUser = _db.Users.FirstOrDefault(u => u.Id == 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
