using CzomPack.Extensions;
using CzSoft.Database.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CzSoft.Database;

public class CzSoftDatabaseContext : DbContext
{
    public string ConnectionString { get; } = null;
    public CzSoftDatabaseContext(string connectionString) : base()
    {
        ConnectionString = connectionString;
    }

    public CzSoftDatabaseContext([NotNull] DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(e => e.Authors).HasConversion(to => to.Count == 0 ? null : Authors.Parse(to).FromXml(), from => string.IsNullOrEmpty(from) ? new() : from.ToXml<Authors>().Author);
        modelBuilder.Entity<Product>().Property(e => e.Dependencies).HasConversion(to => to.Count == 0 ? null : Dependencies.Parse(to).FromXml(), from => string.IsNullOrEmpty(from) ? new() : from.ToXml<Dependencies>().Dependency.Select(d => ProductVersions.First(pv => pv.Id == d.VersionId)).ToList());
        modelBuilder.Entity<Product>().Property(e => e.Tags).HasConversion(to => to.Count == 0 ? null : Tags.Parse(to).FromXml(), from => string.IsNullOrEmpty(from) ? new() : from.ToXml<Tags>().Tag.ToList());

    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVersion> ProductVersions { get; set; }
}
