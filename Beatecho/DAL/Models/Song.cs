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
                var songImgFromAlb = AlbumSongs.FirstOrDefault();
                if (songImgFromAlb != null)
                {
                    return songImgFromAlb.Album.Photo;
                }
                return "D:\\проекты вс\\Beatecho\\Beatecho\\Wins\\1.jpg";
            }
        }
        public virtual ICollection<AlbumSongs> AlbumSongs { get; set; } = new List<AlbumSongs>();
        public virtual ICollection<SongGenre> SongGenres { get; set; } = new List<SongGenre>();
    }
}
