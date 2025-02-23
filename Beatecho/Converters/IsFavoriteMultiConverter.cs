using Beatecho.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Beatecho.Converters
{
    public class IsFavoriteMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int songId && values[1] is object viewModel)
            {
                switch (viewModel)
                {
                    case AlbumPageViewModel albumViewModel:
                        return albumViewModel.IsSongFavorite(songId);
                    case ArtistViewModel artistViewModel:
                        return artistViewModel.IsSongFavorite(songId);
                    case PlaylistPageViewModel playlistViewModel:
                        return playlistViewModel.IsSongFavorite(songId);
                    default:
                        return false;
                }
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
