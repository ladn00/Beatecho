using Beatecho.DAL;
using Beatecho.DAL.Models;
using Beatecho.Views.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Beatecho.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand GoToArtistCommand;

        public MainWindowViewModel()
        {
            GoToArtistCommand = new RelayCommand<string>(GoToArtist);
        }

        public void GoToArtist(string artist)
        {
            
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
