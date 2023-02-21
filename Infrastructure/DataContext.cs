using rny_Testtask2.models;
using Microsoft.EntityFrameworkCore;

namespace rny_Testtask2.Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}
