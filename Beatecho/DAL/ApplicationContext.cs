using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Beatecho.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Beatecho.DAL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Song> Songs => Set<Song>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Album> Albums => Set<Album>();
        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<UserType> UserTypes => Set<UserType>();
        public DbSet<SongGenre> SongGenres => Set<SongGenre>();
        public DbSet<AlbumSongs> AlbumSongs => Set<AlbumSongs>();

        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(DbHelper.ConnString);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AlbumSongs>()
       .HasKey(als => new { als.SongId, als.AlbumId });

            // Связь между AlbumSongs и Song
            modelBuilder.Entity<AlbumSongs>()
                .HasOne(als => als.Song)
                .WithMany(s => s.AlbumSongs)
                .HasForeignKey(als => als.SongId);

            // Связь между AlbumSongs и Album
            modelBuilder.Entity<AlbumSongs>()
                .HasOne(als => als.Album)
                .WithMany(a => a.AlbumSongs)
                .HasForeignKey(als => als.AlbumId);

            // Составной ключ для ArtistAlbums
            modelBuilder.Entity<ArtistAlbums>()
                .HasKey(aa => new { aa.ArtistId, aa.AlbumId });

            // Связь между ArtistAlbums и Artist
            modelBuilder.Entity<ArtistAlbums>()
                .HasOne(aa => aa.Artist)
                .WithMany(a => a.ArtistAlbums)
                .HasForeignKey(aa => aa.ArtistId);

            // Связь между ArtistAlbums и Album
            modelBuilder.Entity<ArtistAlbums>()
                .HasOne(aa => aa.Album)
                .WithMany(a => a.ArtistAlbums)
                .HasForeignKey(aa => aa.AlbumId);

            // Составной ключ для SongGenre
            modelBuilder.Entity<SongGenre>()
                .HasKey(sg => new { sg.SongId, sg.GenreId });

            // Связь между SongGenre и Song
            modelBuilder.Entity<SongGenre>()
                .HasOne(sg => sg.Song)
                .WithMany(s => s.SongGenres)
                .HasForeignKey(sg => sg.SongId);

            // Связь между SongGenre и Genre
            modelBuilder.Entity<SongGenre>()
                .HasOne(sg => sg.Genre)
                .WithMany(g => g.SongGenres)
                .HasForeignKey(sg => sg.GenreId);

            // Связь между User и UserType
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserType)
                .WithMany(ut => ut.Users)
                .HasForeignKey(u => u.UserTypeId);
        }
    }
}
