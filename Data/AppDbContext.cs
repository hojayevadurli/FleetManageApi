using FleetManage.Api.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FleetManage.Api.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Truck> Trucks => Set<Truck>();
        public DbSet<Trailer> Trailers => Set<Trailer>();

        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
        public DbSet<WorkOrderLine> WorkOrderLines => Set<WorkOrderLine>();

        public DbSet<Document> Documents => Set<Document>();
        public DbSet<DocumentLink> DocumentLinks => Set<DocumentLink>();

        private readonly Guid _tenantId; // captured from ITenantContext

        public DbSet<ServiceHistory> ServiceHistories => Set<ServiceHistory>();
        public DbSet<ServiceHistoryLine> ServiceHistoryLines => Set<ServiceHistoryLine>();

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ITenantContext? tenantContext) : base(options)
        {
            _tenantId = tenantContext?.TenantId ?? Guid.Empty;
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ----------------------------
            // Relationships / constraints
            // ----------------------------

            // User ↔ Tenant relationship
            b.Entity<AppUser>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Per-tenant unique equipment numbers
            b.Entity<Truck>()
                .HasIndex(t => new { t.TenantId, t.Number })
                .IsUnique();

            b.Entity<Trailer>()
                .HasIndex(t => new { t.TenantId, t.Number })
                .IsUnique();

            // WorkOrder -> Lines (cascade delete)
            // ✅ Make the relationship optional to avoid EF warning with WorkOrder global filters
            b.Entity<WorkOrderLine>()
                .HasOne(x => x.WorkOrder)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // Document jsonb (Postgres)
            b.Entity<Document>()
                .Property(x => x.ExtractedJson)
                .HasColumnType("jsonb");

            // ----------------------------
            // DocumentLink
            // ----------------------------

            b.Entity<DocumentLink>()
                .HasKey(x => new { x.TenantId, x.Id });

            b.Entity<DocumentLink>()
                .HasIndex(x => new { x.TenantId, x.DocumentId, x.EntityType, x.EntityId })
                .IsUnique();

            b.Entity<DocumentLink>()
                .HasOne(x => x.Document)
                .WithMany()
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----------------------------
            // Explicit mappings (PascalCase columns / tables)
            // ----------------------------

            b.Entity<WorkOrder>(e =>
            {
                e.ToTable("WorkOrders");
                e.HasKey(x => x.Id);

                e.Property(x => x.TenantId).HasColumnName("TenantId");

                e.Property(x => x.AssetType).HasColumnName("AssetType");
                e.Property(x => x.AssetId).HasColumnName("AssetId");

                e.Property(x => x.VendorId).HasColumnName("VendorId");
                e.Property(x => x.WoNumber).HasColumnName("WoNumber");

                e.Property(x => x.Odometer).HasColumnName("Odometer");
                e.Property(x => x.ServiceDate).HasColumnName("ServiceDate");

                e.Property(x => x.Summary).HasColumnName("Summary");

                e.Property(x => x.TotalAmount).HasColumnName("TotalAmount");
                e.Property(x => x.TaxAmount).HasColumnName("TaxAmount");

                e.Property(x => x.Status).HasColumnName("Status");

                e.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
                e.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");

                e.Property(x => x.DeletedAt).HasColumnName("DeletedAt");
                e.Property(x => x.DeletedStatus).HasColumnName("DeletedStatus");
            });

            b.Entity<WorkOrderLine>(e =>
            {
                e.ToTable("WorkOrderLines");
                e.HasKey(x => x.Id);

                // ✅ WorkOrderLine now inherits TenantEntity
                e.Property(x => x.TenantId).HasColumnName("TenantId");

                e.Property(x => x.WorkOrderId).HasColumnName("WorkOrderId");
                e.Property(x => x.Type).HasColumnName("Type");
                e.Property(x => x.Description).HasColumnName("Description");
                e.Property(x => x.Qty).HasColumnName("Qty");
                e.Property(x => x.UnitPrice).HasColumnName("UnitPrice");
                e.Property(x => x.Amount).HasColumnName("Amount");
                e.Property(x => x.PartNumber).HasColumnName("PartNumber");
            });

            b.Entity<Document>(e =>
            {
                e.ToTable("Documents");
                e.HasKey(x => x.Id);

                e.Property(x => x.TenantId).HasColumnName("TenantId");
                e.Property(x => x.ExtractedJson)
                    .HasColumnType("jsonb")
                    .HasColumnName("ExtractedJson");
            });

            b.Entity<DocumentLink>(e =>
            {
                e.ToTable("DocumentLinks");

                e.Property(x => x.TenantId).HasColumnName("TenantId");
                e.Property(x => x.Id).HasColumnName("Id");
                e.Property(x => x.DocumentId).HasColumnName("DocumentId");
                e.Property(x => x.EntityType).HasColumnName("EntityType");
                e.Property(x => x.EntityId).HasColumnName("EntityId");
                e.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
            });

            // ----------------------------
            // Global query filters (tenant + soft delete)
            // IMPORTANT:
            // - Use "_tenantId == Guid.Empty || ..." to avoid OR boolean translation issues
            // - No navigation properties in filters
            // ----------------------------

            b.Entity<Truck>()
                .HasQueryFilter(t => _tenantId == Guid.Empty || t.TenantId == _tenantId);

            b.Entity<Trailer>()
                .HasQueryFilter(t => _tenantId == Guid.Empty || t.TenantId == _tenantId);

            b.Entity<WorkOrder>()
                .HasQueryFilter(w =>
                    (_tenantId == Guid.Empty || w.TenantId == _tenantId) &&
                    (w.DeletedStatus == null || w.DeletedStatus == 0)
                );

            // ✅ Now safe because WorkOrderLine has its own TenantId (no navigation)
            b.Entity<WorkOrderLine>()
                .HasQueryFilter(l => _tenantId == Guid.Empty || l.TenantId == _tenantId);

            b.Entity<Document>()
                .HasQueryFilter(d => _tenantId == Guid.Empty || d.TenantId == _tenantId);

            b.Entity<DocumentLink>()
                .HasQueryFilter(l => _tenantId == Guid.Empty || l.TenantId == _tenantId);

            b.Entity<ServiceHistory>()
      .HasQueryFilter(x => x.DeletedStatus == 0);


            // Intentionally NO filter on Tenant or AppUser
        }

        public override int SaveChanges()
        {
            InjectTenantIds();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            InjectTenantIds();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void InjectTenantIds()
        {
            if (_tenantId == Guid.Empty) return;

            var entries = ChangeTracker.Entries<TenantEntity>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                if (entry.Entity.TenantId == Guid.Empty)
                {
                    entry.Entity.TenantId = _tenantId;
                }
            }
        }
    }
}
