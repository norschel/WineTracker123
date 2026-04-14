using System.IO;
using Microsoft.EntityFrameworkCore;
using WineTracker.Models;

namespace WineTracker.Data;

public class WineDbContext : DbContext
{
    public DbSet<Wine> Wines => Set<Wine>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var folder = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "WineTracker");

        Directory.CreateDirectory(folder);

        var dbPath = Path.Combine(folder, "wines.db");
        options.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wine>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.Property(w => w.Name).IsRequired();
            entity.Property(w => w.Rating).HasDefaultValue(1);
        });
    }
}
