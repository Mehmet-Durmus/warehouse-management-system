using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Domain.Entities;
using WHMS.Domain.Entities.Abstractions;
using WHMS.Persistence.Extensions;

namespace WHMS.Persistence.Contexts;

public class WHMSDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly ICurrentUserService? _currentUserService;
    public WHMSDbContext(DbContextOptions options, ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<City> Cities { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Neighborhood> Neighborhoods { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SKU> SKUs { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryItem> DeliveryItems { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentItem> ShipmentItems { get; set; }
    public DbSet<InventoryCount> InventoryCounts { get; set; }
    public DbSet<InventoryCountLine> InventoryCountLines { get; set; }
    public DbSet<WasteRecord> WasteRecords { get; set; }

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

        builder.HasSequence<int>("WarehouseManagerSequence").StartsAt(1);
        builder.HasSequence<int>("WarehouseStaffSequence").StartsAt(1);
            

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

        builder.Entity<Category>(c =>
        {
            c.Property(c => c.CategoryName).HasMaxLength(50);
        });
        
        builder.Entity<SKU>(sku =>
        {
            sku.Property(s => s.SKUName).HasMaxLength(50);
            sku.Property(s => s.Barcode).HasMaxLength(100);
            sku.Property(s => s.UnitPrice).HasColumnType("numeric(7,2)");
        });
        
        builder.Entity<Store>(e =>
        {
            e.Property(s => s.StoreName).HasMaxLength(50);
            e.OwnsOne(w => w.Address, a =>
            {
                a.Property( p => p.CityId).IsRequired();
                a.Property( p => p.DistrictId).IsRequired();
                a.Property( p => p.NeighborhoodId).IsRequired();
                a.Property( p => p.PostalCode).HasMaxLength(10);
                a.Property( p => p.AddressLine).HasMaxLength(500);
            });
        });

        builder.Entity<Delivery>(e =>
        {
            e.Property(d => d.ExpectedArrivalDate).HasColumnType("timestamp without time zone");
            e.HasOne(d => d.Warehouse)
                .WithMany(w => w.Deliveries)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Shipment>(e =>
        {
            e.Property(s => s.ExpectedSendingDate).HasColumnType("timestamp without time zone");
            e.Property(s => s.SendingDate).HasColumnType("timestamp without time zone");
            e.HasOne(s => s.Warehouse)
                .WithMany(w => w.Shipments)
                .HasForeignKey(s => s.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(s => s.Store)
                .WithMany(st => st.Shipments)
                .HasForeignKey(s => s.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        });
            
        
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
                        auditable.Entity.CreatedById = _currentUserService!.UserId!;
                        auditable.Entity.CreatedByName = _currentUserService.FullName!;
                        auditable.Entity.CreatedByUserName = _currentUserService.UserName!;
                        auditable.Entity.UpdatedAt = DateTime.Now;
                        auditable.Entity.UpdatedById = _currentUserService.UserId;
                        auditable.Entity.UpdatedByName = _currentUserService.FullName!;
                        auditable.Entity.UpdatedByUserName = _currentUserService.UserName!;
                        break;
                    case EntityState.Modified:
                        auditable.Entity.UpdatedAt = DateTime.Now;
                        auditable.Entity.UpdatedById = _currentUserService!.UserId;
                        auditable.Entity.UpdatedByName = _currentUserService.FullName!;
                        auditable.Entity.UpdatedByUserName = _currentUserService.UserName!;
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