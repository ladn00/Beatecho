using System;
using System.Collections.Generic;
using System.Linq;
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
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<UserType> UserTypes => Set<UserType>();
        public DbSet<SongGenre> SongGenres => Set<SongGenre>();

        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(DbHelper.ConnString);
        }
    }
}
