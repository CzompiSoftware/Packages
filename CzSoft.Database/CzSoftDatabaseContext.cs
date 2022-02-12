using CzSoft.Database.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CzSoft.Database
{
    public class CzSoftDatabaseContext : DbContext
    {
        public CzSoftDatabaseContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //use this to configure the context

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //use this to configure the model
            
        }

        //public DbSet<Package> Packages { get; set; }
        public DbSet<Product> Products { get; set; }
        //public DbSet<Mod> Mods { get; set; }
        //public DbSet<ModVersion> ModVersions { get; set; }
        //public DbSet<PackNewsItem> PackNews { get; set; }
        //public DbSet<PackVersion> PackVersions { get; set; }
        //public DbSet<PackDependency> PackDependencies { get; set; }
    }
}