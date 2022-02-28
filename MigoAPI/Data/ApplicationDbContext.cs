using Microsoft.EntityFrameworkCore;
using MigoAPI.Models;

namespace MigoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ListDetail> ListDetails { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<TrackDetail> TrackDetails { get; set; }
    }
}
