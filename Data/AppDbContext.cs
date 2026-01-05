using FleetManage.Api.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FleetManage.Api.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Tenant> Tenants => Set<Tenant>();
        // Combined Equipment Table
        public DbSet<Equipment> Equipments => Set<Equipment>(); 

        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
        public DbSet<WorkOrderLineItem> WorkOrderLineItems => Set<WorkOrderLineItem>();
        public DbSet<WorkOrderDocument> WorkOrderDocuments => Set<WorkOrderDocument>();

        public DbSet<RecallCampaign> RecallCampaigns => Set<RecallCampaign>();
        public DbSet<EquipmentRecall> EquipmentRecalls => Set<EquipmentRecall>();

        public DbSet<Document> Documents => Set<Document>();

        public DbSet<DocumentLink> DocumentLinks => Set<DocumentLink>();
        public DbSet<EquipmentDocument> EquipmentDocuments => Set<EquipmentDocument>();

        private readonly Guid _tenantId; // captured from ITenantContext



        public DbSet<ServicePartner> ServicePartners => Set<ServicePartner>();
        public DbSet<ServicePartnerRating> ServicePartnerRatings => Set<ServicePartnerRating>();
        public DbSet<Industry> Industries => Set<Industry>();
        public DbSet<FleetCategory> FleetCategories => Set<FleetCategory>(); 
        public DbSet<EquipmentType> EquipmentTypes => Set<EquipmentType>();

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
            // Seeding Industries
            // ----------------------------
            b.Entity<Industry>().HasData(
                new Industry { Id = 1, Name = "Construction" },
                new Industry { Id = 2, Name = "Trucking & Logistics" },
                new Industry { Id = 3, Name = "School Transportation" }
            );

            // ----------------------------
            // Seeding FleetCategories
            // ----------------------------
            b.Entity<FleetCategory>().HasData(
                // Industry 1 - Construction
                new FleetCategory { Id = 1, IndustryId = 1, Code = "MOVE", Name = "Earthmoving", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 2, IndustryId = 1, Code = "PAVE", Name = "Road & Paving", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 3, IndustryId = 1, Code = "CONC", Name = "Concrete", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 4, IndustryId = 1, Code = "UTIL", Name = "Utilities", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 5, IndustryId = 1, Code = "DEM", Name = "Demolition", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 6, IndustryId = 1, Code = "SITE", Name = "Site Logistics", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 7, IndustryId = 1, Code = "SERV", Name = "Service / Support", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 8, IndustryId = 1, Code = "SPARE", Name = "Spare / Backup", IsActive = true, ListEquipment = true },

                // Industry 2 - Trucking & Logistics
                new FleetCategory { Id = 9, IndustryId = 2, Code = "LINE", Name = "Linehaul", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 10, IndustryId = 2, Code = "LOCAL", Name = "Local / P&D", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 11, IndustryId = 2, Code = "DED", Name = "Dedicated", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 12, IndustryId = 2, Code = "YARD", Name = "Yard / Spotter", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 13, IndustryId = 2, Code = "DRAY", Name = "Intermodal / Drayage", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 14, IndustryId = 2, Code = "SPARE", Name = "Spare / Backup", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 15, IndustryId = 2, Code = "OO", Name = "Owner-Op (Managed)", IsActive = true, ListEquipment = true },

                // Industry 3 - School Transportation
                new FleetCategory { Id = 16, IndustryId = 3, Code = "ROUTE", Name = "AM / PM Routes", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 17, IndustryId = 3, Code = "SPEC", Name = "Special Needs", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 18, IndustryId = 3, Code = "FIELD", Name = "Field Trips / Charter", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 19, IndustryId = 3, Code = "ACT", Name = "Activity Buses", IsActive = true, ListEquipment = true },
                new FleetCategory { Id = 20, IndustryId = 3, Code = "SPARE", Name = "Spare / Relief", IsActive = true, ListEquipment = true }
            );

            // ----------------------------
            // Seeding EquipmentTypes
            // ----------------------------
            b.Entity<EquipmentType>().HasData(
                // -- Trucking (1000s) --
                new EquipmentType { Id = 1001, IndustryId = 2, FleetCategoryId = 9, Name = "Tractor (Semi Truck)", Code = "TRCTR", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1002, IndustryId = 2, FleetCategoryId = 9, Name = "Day Cab Truck", Code = "DAY", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1003, IndustryId = 2, FleetCategoryId = 9, Name = "Sleeper Truck", Code = "SLP", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1004, IndustryId = 2, FleetCategoryId = 10, Name = "Straight Truck", Code = "STR", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1005, IndustryId = 2, FleetCategoryId = 10, Name = "Box Truck", Code = "BOX", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1006, IndustryId = 2, FleetCategoryId = 10, Name = "Cargo Van", Code = "VAN", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1007, IndustryId = 2, FleetCategoryId = 10, Name = "Pickup Truck", Code = "PUP", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },

                new EquipmentType { Id = 1101, IndustryId = 2, FleetCategoryId = 9, Name = "Dry Van Trailer", Code = "DV", MeterMode = 0, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1102, IndustryId = 2, FleetCategoryId = 9, Name = "Refrigerated Trailer (Reefer)", Code = "RFR", MeterMode = 0, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1103, IndustryId = 2, FleetCategoryId = 9, Name = "Flatbed Trailer", Code = "FLT", MeterMode = 0, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1104, IndustryId = 2, FleetCategoryId = 9, Name = "Step Deck Trailer", Code = "STD", MeterMode = 0, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1105, IndustryId = 2, FleetCategoryId = 9, Name = "Lowboy Trailer", Code = "LBY", MeterMode = 0, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1106, IndustryId = 2, FleetCategoryId = 9, Name = "Tanker Trailer", Code = "TNK", MeterMode = 0, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1107, IndustryId = 2, FleetCategoryId = 9, Name = "Dump Trailer", Code = "DMP", MeterMode = 0, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1108, IndustryId = 2, FleetCategoryId = 9, Name = "Intermodal Chassis", Code = "CHS", MeterMode = 0, HasVin = true, HasSerial = false, IsActive = true },

                new EquipmentType { Id = 1201, IndustryId = 2, FleetCategoryId = 14, Name = "Service Truck", Code = "SVC", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1202, IndustryId = 2, FleetCategoryId = 14, Name = "Mobile Mechanic Truck", Code = "MMT", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1203, IndustryId = 2, FleetCategoryId = 14, Name = "Tire Service Unit", Code = "TIRE", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1204, IndustryId = 2, FleetCategoryId = 14, Name = "Fuel Service Truck", Code = "FUEL", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },

                new EquipmentType { Id = 1301, IndustryId = 2, FleetCategoryId = 12, Name = "Yard Tractor (Spotter)", Code = "YRD", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1302, IndustryId = 2, FleetCategoryId = 12, Name = "Terminal Tractor", Code = "TRM", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 1303, IndustryId = 2, FleetCategoryId = 12, Name = "Generator", Code = "GEN", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 1304, IndustryId = 2, FleetCategoryId = 12, Name = "Light Tower", Code = "LGT", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 1305, IndustryId = 2, FleetCategoryId = 12, Name = "Mobile Office Trailer", Code = "OFF", MeterMode = 0, HasVin = false, HasSerial = true, IsActive = true },

                // -- Construction (2000s) --
                new EquipmentType { Id = 2001, IndustryId = 1, FleetCategoryId = 1, Name = "Excavator", Code = "EXC", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2002, IndustryId = 1, FleetCategoryId = 1, Name = "Mini Excavator", Code = "MEX", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2003, IndustryId = 1, FleetCategoryId = 1, Name = "Bulldozer", Code = "DOZ", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2004, IndustryId = 1, FleetCategoryId = 1, Name = "Wheel Loader", Code = "WLD", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2005, IndustryId = 1, FleetCategoryId = 1, Name = "Skid Steer Loader", Code = "SSL", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2006, IndustryId = 1, FleetCategoryId = 1, Name = "Compact Track Loader", Code = "CTL", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2007, IndustryId = 1, FleetCategoryId = 1, Name = "Backhoe Loader", Code = "BHL", MeterMode = 3, HasVin = true, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2008, IndustryId = 1, FleetCategoryId = 1, Name = "Motor Grader", Code = "GRD", MeterMode = 3, HasVin = true, HasSerial = true, IsActive = true },

                new EquipmentType { Id = 2101, IndustryId = 1, FleetCategoryId = 7, Name = "Forklift", Code = "FLK", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2102, IndustryId = 1, FleetCategoryId = 7, Name = "Rough Terrain Forklift", Code = "RTF", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2103, IndustryId = 1, FleetCategoryId = 7, Name = "Telehandler", Code = "TEL", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2104, IndustryId = 1, FleetCategoryId = 7, Name = "Mobile Crane", Code = "CRN", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2105, IndustryId = 1, FleetCategoryId = 7, Name = "Tower Crane", Code = "TCR", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2106, IndustryId = 1, FleetCategoryId = 7, Name = "Boom Lift", Code = "BML", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2107, IndustryId = 1, FleetCategoryId = 7, Name = "Scissor Lift", Code = "SCL", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },

                new EquipmentType { Id = 2201, IndustryId = 1, FleetCategoryId = 2, Name = "Road Roller (Compactor)", Code = "ROL", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2202, IndustryId = 1, FleetCategoryId = 2, Name = "Asphalt Paver", Code = "PAV", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2203, IndustryId = 1, FleetCategoryId = 2, Name = "Cold Planer (Milling)", Code = "MIL", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2204, IndustryId = 1, FleetCategoryId = 2, Name = "Soil Compactor", Code = "SCP", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2205, IndustryId = 1, FleetCategoryId = 2, Name = "Plate Compactor", Code = "PLC", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },

                new EquipmentType { Id = 2301, IndustryId = 1, FleetCategoryId = 3, Name = "Concrete Mixer Truck", Code = "CMT", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 2302, IndustryId = 1, FleetCategoryId = 3, Name = "Concrete Pump Truck", Code = "CPT", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 2303, IndustryId = 1, FleetCategoryId = 3, Name = "Concrete Batching Plant", Code = "CBP", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2304, IndustryId = 1, FleetCategoryId = 3, Name = "Power Trowel", Code = "PTR", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2305, IndustryId = 1, FleetCategoryId = 3, Name = "Concrete Saw", Code = "CSW", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },

                new EquipmentType { Id = 2401, IndustryId = 1, FleetCategoryId = 4, Name = "Generator", Code = "GEN", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2402, IndustryId = 1, FleetCategoryId = 4, Name = "Air Compressor", Code = "ACM", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2403, IndustryId = 1, FleetCategoryId = 4, Name = "Light Tower", Code = "LGT", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2404, IndustryId = 1, FleetCategoryId = 4, Name = "Welding Machine", Code = "WLDG", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2405, IndustryId = 1, FleetCategoryId = 4, Name = "Pressure Washer", Code = "PWSH", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },

                new EquipmentType { Id = 2501, IndustryId = 1, FleetCategoryId = 5, Name = "Demolition Excavator", Code = "DEX", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2502, IndustryId = 1, FleetCategoryId = 5, Name = "Hydraulic Breaker", Code = "HBR", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2503, IndustryId = 1, FleetCategoryId = 5, Name = "Crusher", Code = "CRS", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2504, IndustryId = 1, FleetCategoryId = 5, Name = "Screener", Code = "SCR", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },

                new EquipmentType { Id = 2601, IndustryId = 1, FleetCategoryId = 6, Name = "Dump Truck", Code = "DMP", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 2602, IndustryId = 1, FleetCategoryId = 6, Name = "Articulated Dump Truck", Code = "ADT", MeterMode = 2, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2603, IndustryId = 1, FleetCategoryId = 6, Name = "Water Truck", Code = "WTR", MeterMode = 1, HasVin = true, HasSerial = false, IsActive = true },
                new EquipmentType { Id = 2604, IndustryId = 1, FleetCategoryId = 6, Name = "Site Trailer", Code = "STRL", MeterMode = 0, HasVin = false, HasSerial = true, IsActive = true },
                new EquipmentType { Id = 2605, IndustryId = 1, FleetCategoryId = 6, Name = "Mobile Office", Code = "MOFF", MeterMode = 0, HasVin = false, HasSerial = true, IsActive = true }
            );

            // ----------------------------
            // Relationships / constraints
            // ----------------------------

            // User ↔ Tenant relationship
            b.Entity<AppUser>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tenant ↔ Industry (Optional)
            b.Entity<Tenant>()
                .HasOne(t => t.Industry)
                .WithMany()
                .HasForeignKey(t => t.IndustryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Equipment unique index per tenant
            b.Entity<Equipment>()
                .HasIndex(x => new { x.TenantId, x.UnitNumber })
                .IsUnique();
            
            // WorkOrder -> Equipment relationship
            b.Entity<WorkOrder>()
                .HasOne(x => x.Equipment)
                .WithMany(x => x.WorkOrders)
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // WorkOrder -> Vendor (ServicePartner)
            b.Entity<WorkOrder>()
                .HasOne(x => x.Vendor)
                .WithMany(x => x.WorkOrders)
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.SetNull);



            // WorkOrder -> Lines (cascade delete)
            b.Entity<WorkOrderLineItem>()
                .HasOne(x => x.WorkOrder)
                .WithMany(x => x.LineItems)
                .HasForeignKey(x => x.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);



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
            // Explicit mappings
            // ----------------------------

            b.Entity<ServicePartner>(e =>
            {
                e.ToTable("ServicePartners");
                e.HasKey(x => x.Id);
                e.Property(x => x.Specialties)
                 .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => string.IsNullOrEmpty(v) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                 )
                 .HasColumnType("jsonb");
            });

            b.Entity<ServicePartnerRating>(e =>
            {
                e.ToTable("ServicePartnerRatings");
                e.HasKey(x => x.Id);
                e.HasOne(x => x.ServicePartner)
                    .WithMany(x => x.Ratings)
                    .HasForeignKey(x => x.ServicePartnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<WorkOrder>(e =>
            {
                e.ToTable("WorkOrders");
                e.HasKey(x => x.Id);

                e.Property(x => x.TenantId).HasColumnName("TenantId");
                e.Property(x => x.EquipmentId).HasColumnName("EquipmentId");
                e.Property(x => x.VendorId).HasColumnName("VendorId");
                e.Property(x => x.WorkOrderNumber).HasColumnName("WorkOrderNumber");

                e.Property(x => x.Status)
                    .HasConversion<string>() // Store Enum as string for readability or int? User didn't specify. String is safer for enums usually. Or int. Let's use string.
                    .HasColumnName("Status");
                
                e.Property(x => x.Priority)
                    .HasConversion<string>()
                    .HasColumnName("Priority");

                e.Property(x => x.CostSource)
                     .HasConversion<string>();

                e.Property(x => x.DeletedAt).HasColumnName("DeletedAt");
                e.Property(x => x.IsDeleted).HasColumnName("IsDeleted");
            });

            b.Entity<WorkOrderLineItem>(e =>
            {
                e.ToTable("WorkOrderLineItems");
                e.HasKey(x => x.Id);
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

            b.Entity<EquipmentDocument>(e =>
            {
                e.ToTable("EquipmentDocuments");
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.EquipmentId, x.DocumentId }).IsUnique();

                e.HasOne(x => x.Equipment)
                    .WithMany(x => x.EquipmentDocuments)
                    .HasForeignKey(x => x.EquipmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Document)
                    .WithMany(x => x.EquipmentDocuments)
                    .HasForeignKey(x => x.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<WorkOrderDocument>(e =>
            {
                e.ToTable("WorkOrderDocuments");
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.WorkOrderId, x.DocumentId }).IsUnique();

                e.HasOne(x => x.WorkOrder)
                    .WithMany(x => x.WorkOrderDocuments)
                    .HasForeignKey(x => x.WorkOrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Document)
                    .WithMany(x => x.WorkOrderDocuments) // Assuming inverse is added or use WithMany()
                    .HasForeignKey(x => x.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<RecallCampaign>(e =>
            {
                e.HasIndex(x => x.Code).IsUnique();
            });

            b.Entity<EquipmentRecall>(e =>
            {
                e.HasOne(x => x.Equipment)
                 .WithMany(x => x.Recalls)
                 .HasForeignKey(x => x.EquipmentId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ----------------------------
            // Global query filters
            // ----------------------------

            b.Entity<Equipment>()
                .HasQueryFilter(e => 
                    (_tenantId == Guid.Empty || e.TenantId == _tenantId) && !e.IsDeleted
                );

            b.Entity<WorkOrder>()
                .HasQueryFilter(w =>
                    (_tenantId == Guid.Empty || w.TenantId == _tenantId) &&
                    !w.IsDeleted
                );

            // Safe because WorkOrderLineItem has its own TenantId
            b.Entity<WorkOrderLineItem>()
                .HasQueryFilter(l => _tenantId == Guid.Empty || l.TenantId == _tenantId);



            b.Entity<Document>()
                .HasQueryFilter(d => _tenantId == Guid.Empty || d.TenantId == _tenantId);

            b.Entity<DocumentLink>()
                .HasQueryFilter(l => _tenantId == Guid.Empty || l.TenantId == _tenantId);

            b.Entity<EquipmentDocument>()
                .HasQueryFilter(x => _tenantId == Guid.Empty || x.TenantId == _tenantId);

            b.Entity<WorkOrderDocument>()
                .HasQueryFilter(x => _tenantId == Guid.Empty || x.TenantId == _tenantId);



            b.Entity<ServicePartner>()
                .HasQueryFilter(x => _tenantId == Guid.Empty || x.TenantId == _tenantId);

            b.Entity<ServicePartnerRating>()
                .HasQueryFilter(x => _tenantId == Guid.Empty || x.TenantId == _tenantId);


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
