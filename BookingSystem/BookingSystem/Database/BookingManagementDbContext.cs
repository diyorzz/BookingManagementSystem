using ActualLab.Fusion.Authentication.Services;
using ActualLab.Fusion.EntityFramework;
using ActualLab.Fusion.EntityFramework.Operations;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Database;

public class BookingManagementDbContext : DbContextBase
{
    public BookingManagementDbContext(DbContextOptions options) : base(options) 
    {
        Database.Migrate();    
    }

    // App's own tables
    public DbSet<Ticket> Tickets { get; protected set; } = null!;
    // ActualLab.Fusion.EntityFramework tables
    public DbSet<DbUser<string>> Users { get; protected set; } = null!;
    public DbSet<DbUserIdentity<string>> UserIdentities { get; protected set; } = null!;
    public DbSet<DbSessionInfo<string>> Sessions { get; protected set; } = null!;

    // ActualLab.Fusion.EntityFramework.Operations tables
    public DbSet<DbOperation> Operations { get; protected set; } = null!;
    public DbSet<DbEvent> Events { get; protected set; } = null!;
}
