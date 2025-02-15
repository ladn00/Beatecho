using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Beatecho.DAL.Models
{
    public class Song : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int Duration { get; set; }
        public TimeSpan CreatedAt { get; set; }
        public string? Link { get; set; }

        public string Photo
        {
            get
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var songWithAlbumSongs = db.Songs
                        .Include(s => s.AlbumSongs)
                        .FirstOrDefault(s => s.Id == Id);

                    var songImgFromAlb = songWithAlbumSongs?.AlbumSongs.FirstOrDefault();


                    if (songImgFromAlb != null)
                    {
                        return songImgFromAlb.Album.Photo;
                    }
                    return "D:\\проекты вс\\Beatecho\\Beatecho\\Wins\\1.jpg";
                }
            }
        }

        public string Artist
        {
            get
            {
                string result = "n/a";
                using (ApplicationContext db = new ApplicationContext())
                {
                    var songWithAlbumSongs = db.Songs
                        .Include(s => s.AlbumSongs)
                        .FirstOrDefault(s => s.Id == Id);

                    var songImgFromAlb = songWithAlbumSongs?.AlbumSongs.FirstOrDefault();
                    ArtistAlbums artistAlbums;

                    if (songImgFromAlb != null)
                    {
                        artistAlbums = songImgFromAlb.Album.ArtistAlbums?.FirstOrDefault();

                        if (artistAlbums != null)
                        {
                            result = artistAlbums.Artist.Name;
                        }
                    }
                }
                return result;
            }
        }

        public int TrackNumber
        {
            get
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var songWithAlbumSongs = db.Songs
                        .Include(s => s.AlbumSongs)
                        .FirstOrDefault(s => s.Id == Id);

                    var songImgFromAlb = songWithAlbumSongs?.AlbumSongs.FirstOrDefault();


                    if (songImgFromAlb != null)
                    {
                        return songImgFromAlb.TrackNum;
                    }
                    return 0;
                }
            }
        }

        public virtual ICollection<AlbumSongs> AlbumSongs { get; set; } = new List<AlbumSongs>();
        public virtual ICollection<SongGenre> SongGenres { get; set; } = new List<SongGenre>();
        public virtual ICollection<PlaylistSongs> PlaylistSongs { get; set; } = new List<PlaylistSongs>();
        public virtual ICollection<FavoriteTracks> FavoriteTracks { get; set; } = new List<FavoriteTracks>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
