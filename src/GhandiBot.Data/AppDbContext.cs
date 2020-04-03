using System;
using System.Threading;
using System.Threading.Tasks;
using GhandiBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GhandiBot.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Logs> Logs { get; set; }
        public DbSet<FeatureOverride> FeatureOverride { get; set; }

        public override int SaveChanges()
        {
            GetCreatedAtAndUpdatedAt();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            GetCreatedAtAndUpdatedAt();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            GetCreatedAtAndUpdatedAt();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            GetCreatedAtAndUpdatedAt();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void GetCreatedAtAndUpdatedAt()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                var modelBase = entry.Entity as ModelBase;
                if (modelBase == null) return;

                var now = DateTime.UtcNow;
                switch (entry.State)
                {
                    case EntityState.Added:
                        modelBase.CreatedAt = now;
                        modelBase.UpdatedAt = now;
                        break;
                    case EntityState.Modified:
                        modelBase.UpdatedAt = now;
                        break;
                }
            }
        }
    }
}