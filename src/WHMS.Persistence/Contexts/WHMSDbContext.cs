using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WHMS.Domain.Entities;

namespace WHMS.Persistence.Contexts;

public class WHMSDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public WHMSDbContext(DbContextOptions options) : base (options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Warehouse)
            .WithMany(w => w.ApplicationUsers)
            .HasForeignKey(u => u.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ApplicationUser>(user => 
            user.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone"));
        builder.Entity<ApplicationUser>(user => 
            user.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone"));
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entiries = ChangeTracker.Entries<ApplicationUser>();
        foreach (var entiry in entiries)
        {
            switch (entiry.State)
                {
                    case EntityState.Added:
                        entiry.Entity.CreatedAt = DateTime.Now;
                        entiry.Entity.UpdatedAt = DateTime.Now;
                        entiry.Entity.IsActive = true;
                        break;
                    case EntityState.Modified:
                        entiry.Entity.UpdatedAt = DateTime.Now;
                        break;
                }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}