using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WHMS.Domain.Entities;
using WHMS.Domain.Entities.Abstractions;
using WHMS.Persistence.Extensions;

namespace WHMS.Persistence.Contexts;

public class WHMSDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public WHMSDbContext(DbContextOptions options) : base (options) {}

    public DbSet<City> Cities { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Neighborhood> Neighborhoods { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var auditableTypes = builder.Model.GetEntityTypes()
            .Where(t => typeof(IAuditable).IsAssignableFrom(t.ClrType));

        foreach (var auditableType in auditableTypes)
        {
            builder.Entity(auditableType.ClrType).Property(nameof(IAuditable.CreatedAt))
                .HasColumnType("timestamp without time zone");
            builder.Entity(auditableType.ClrType).Property(nameof(IAuditable.UpdatedAt))
                .HasColumnType("timestamp without time zone");
        }

        builder.AddSoftDeleteQueryFilter();
            

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Warehouse)
            .WithMany(w => w.ApplicationUsers)
            .HasForeignKey(u => u.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Warehouse>(e =>
        {
            e.OwnsOne(w => w.Address, a =>
            {
                a.Property( p => p.CityId).IsRequired();
                a.Property( p => p.DistrictId).IsRequired();
                a.Property( p => p.NeighborhoodId).IsRequired();
                a.Property( p => p.PostalCode).HasMaxLength(10);
                a.Property( p => p.AddressLine).HasMaxLength(500);
            });
        });

        builder.Entity<Neighborhood>()
            .HasOne(n => n.District)
            .WithMany(d => d.Neighborhoods)
            .HasForeignKey(n => n.DistrictId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<District>()
            .HasOne(d => d.City)
            .WithMany(c => c.Districts)
            .HasForeignKey(d => d.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditables = ChangeTracker.Entries<IAuditable>();
        foreach (var auditable in auditables)
        {
            switch (auditable.State)
                {
                    case EntityState.Added:
                        auditable.Entity.CreatedAt = DateTime.Now;
                        auditable.Entity.UpdatedAt = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        auditable.Entity.UpdatedAt = DateTime.Now;
                        break;
                }
        }

        var deletables = ChangeTracker.Entries<ISoftDeletable>();
        foreach (var deletable in deletables)
        {
            if (deletable.State == EntityState.Added)
                deletable.Entity.IsActive = true;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}