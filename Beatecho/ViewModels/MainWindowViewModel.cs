using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Beatecho.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand OpenFavoritesCommand;
        private User user;

        public MainWindowViewModel() 
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                user = db.Users.FirstOrDefault(u => u.Id == 1);
            }
            OpenFavoritesCommand = new RelayCommand(OpenFavorites);
        }

        private void OpenFavorites()
        {
                var favoritePage = new FavoritesPage(user);

                Views.Wins.UserWindow.frame.NavigationService.Navigate(favoritePage);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
