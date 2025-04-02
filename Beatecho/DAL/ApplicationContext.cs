using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
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
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<PlaylistSongs> PlaylistSongs => Set<PlaylistSongs>();
        public DbSet<PlaylistUsers> PlaylistUsers => Set<PlaylistUsers>();
        public DbSet<ArtistAlbums> ArtistAlbums => Set<ArtistAlbums>();
        public DbSet<FavoriteTracks> FavoriteTracks { get; set; }
        public DbSet<UserRecommendation> UserRecommendations => Set<UserRecommendation>();

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

            modelBuilder.Entity<UserRecommendation>()
            .HasKey(ur => ur.UserId);

            modelBuilder.Entity<UserRecommendation>()
    .Property(ur => ur.Recommendations)
    .HasColumnType("jsonb")
    .HasConversion(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), // List<int> → JSON
        v => JsonSerializer.Deserialize<List<int>>(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ?? new List<int>() // JSON → List<int>
    );

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

            modelBuilder.Entity<PlaylistSongs>()
            .HasKey(ps => new { ps.PlaylistId, ps.SongId });

            // Настройка связей
            modelBuilder.Entity<PlaylistSongs>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId);

            modelBuilder.Entity<PlaylistSongs>()
                .HasOne(ps => ps.Song)
                .WithMany(s => s.PlaylistSongs)
                .HasForeignKey(ps => ps.SongId);

            modelBuilder.Entity<PlaylistUsers>()
            .HasKey(ps => new { ps.PlaylistId, ps.UserId });

            // Настройка связей
            modelBuilder.Entity<PlaylistUsers>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistUsers)
                .HasForeignKey(ps => ps.PlaylistId);

            modelBuilder.Entity<PlaylistUsers>()
                .HasOne(ps => ps.User)
                .WithMany(s => s.PlaylistUsers)
                .HasForeignKey(ps => ps.UserId);

            modelBuilder.Entity<FavoriteTracks>()
            .HasKey(ft => new { ft.UserId, ft.SongId });

            modelBuilder.Entity<FavoriteTracks>()
                .HasOne(ft => ft.User)
                .WithMany(u => u.FavoriteTracks)
                .HasForeignKey(ft => ft.UserId);

            modelBuilder.Entity<FavoriteTracks>()
                .HasOne(ft => ft.Song)
                .WithMany(s => s.FavoriteTracks)
                .HasForeignKey(ft => ft.SongId);

        }
    }
}
