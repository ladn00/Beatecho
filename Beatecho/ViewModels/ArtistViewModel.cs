using Beatecho.DAL.Models;
using Beatecho.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Beatecho.ViewModels
{
    public class ArtistViewModel : INotifyPropertyChanged
    {
        public Artist Artist { get; set; }

        public ArtistViewModel(Artist artist)
        {
            Artist = artist;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
