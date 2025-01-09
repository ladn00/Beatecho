using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatecho.DAL.Models
{
    public class Song
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
                using (ApplicationContext db = new ApplicationContext())
                {
                    /*var artistFromAlb = db.Artists
                        .Include(s => s.ArtistAlbums)
                        .FirstOrDefault(s => s.Id == Id);

                    if (artistFromAlb != null)
                    {
                        var artistAlbums = artistFromAlb.ArtistAlbums.FirstOrDefault();

                        if (artistAlbums != null)
                        {
                            return artistAlbums.Artist.Name;
                        }
                    }*/
                    return "n/a";
                }
            }
        }

        public virtual ICollection<AlbumSongs> AlbumSongs { get; set; } = new List<AlbumSongs>();
        public virtual ICollection<SongGenre> SongGenres { get; set; } = new List<SongGenre>();
        public virtual ICollection<PlaylistSongs> PlaylistSongs { get; set; } = new List<PlaylistSongs>();
    }
}
