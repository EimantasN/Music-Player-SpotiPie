using Microsoft.EntityFrameworkCore;
using Models.BackEnd;

namespace Database
{
    public class SpotyPieIDbContext : DbContext
    {
        public SpotyPieIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<CurrentSong> CurrentSong { get; set; }

        public DbSet<Playlist> Playlist { get; set; }

        public DbSet<Artist> Artists { get; set; }

        public DbSet<Song> Songs { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Song> ActiveSong { get; set; }
    }
}
