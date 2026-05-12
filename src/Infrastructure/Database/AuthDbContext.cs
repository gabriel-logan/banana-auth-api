using Banana.Auth.Api.Modules.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Banana.Auth.Api.Infrastructure.Database;

public class AuthDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
        });
    }

    public override int SaveChanges()
    {
        ApplyAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditFields()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<User>())
        {
            if (entry.State == EntityState.Added)
            {
                SetCreatedAndUpdatedAt(entry, now);
                continue;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Property(user => user.CreatedAt).IsModified = false;
            }
        }
    }

    private static void SetCreatedAndUpdatedAt(EntityEntry<User> entry, DateTime now)
    {
        if (entry.Entity.CreatedAt == default)
        {
            entry.Entity.CreatedAt = now;
        }

        entry.Entity.UpdatedAt = now;
    }
}
