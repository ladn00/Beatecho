using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Beatecho.DAL.Models
{
    public class Album
    {
        public int Id { get; set; }
        public int ReleaseYear { get; set; }
        public string? Title { get; set; }
        public string Photo { get; set; }

        public virtual ICollection<AlbumSongs> AlbumSongs { get; set; } = new List<AlbumSongs>();
        public virtual ICollection<ArtistAlbums> ArtistAlbums { get; set; } = new List<ArtistAlbums>();

        public virtual string Artist
        {
            get
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var AlbumWithArtists = db.Albums
                        .Include(s => s.ArtistAlbums)
                        .FirstOrDefault(s => s.Id == Id);

                    var OneAlbumArtist = AlbumWithArtists?.ArtistAlbums.FirstOrDefault();


                    if (OneAlbumArtist != null)
                    {
                        return OneAlbumArtist.Artist.Name;
                    }
                    return "D:\\проекты вс\\Beatecho\\Beatecho\\Wins\\1.jpg";
                }
            }
        }
    }
}
