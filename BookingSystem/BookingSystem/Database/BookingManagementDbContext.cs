using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BookingSystem.Database
{
    public class BookingManagementDbContext : DbContext
    {
        public DbSet<Ticket> Tickets { get; set; }

        public BookingManagementDbContext(DbContextOptions<BookingManagementDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
