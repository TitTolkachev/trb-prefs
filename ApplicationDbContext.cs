using Microsoft.EntityFrameworkCore;
using trb_prefs.Entities;

namespace trb_prefs;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<HiddenAccount> Accounts { get; set; } = null!;
    public DbSet<Theme> Themes { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HiddenAccount>().HasKey(x => x.Id);
        modelBuilder.Entity<Theme>().HasKey(x => x.Id);
    }
}