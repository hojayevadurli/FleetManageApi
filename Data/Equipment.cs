using FleetManage.Api.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManage.Api.Data
{
    public class Equipment : TenantEntity
    {
        // -------------------------
        // Identity / Tenancy
        // -------------------------
        // Id and TenantId come from TenantEntity base class

        // -------------------------
        // Classification
        // -------------------------
        public int EquipmentTypeId { get; set; }
        public EquipmentType EquipmentType { get; set; } = default!;

        public int FleetCategoryId { get; set; }
        public FleetCategory FleetCategory { get; set; } = default!;

        // -------------------------
        // Identification
        // -------------------------
        public string UnitNumber { get; set; } = default!;
        public string? DisplayName { get; set; }

        public string? Vin { get; set; }
        public string? SerialNumber { get; set; }
        public string? PlateNumber { get; set; }

        // -------------------------
        // Specs
        // -------------------------
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }

        // -------------------------
        // Status
        // -------------------------
        public EquipmentLifecycleStatus LifecycleStatus { get; set; }
        public EquipmentOperationalStatus OperationalStatus { get; set; }

        // -------------------------
        // Meters
        // -------------------------
        public int? OdometerCurrent { get; set; } // Miles/KM
        public int? HoursCurrent { get; set; }    // Engine Hours

        // -------------------------
        // Dates
        // -------------------------
        public DateOnly? AcquiredDate { get; set; }
        public DateOnly? InServiceDate { get; set; }
        public DateOnly? OutOfServiceDate { get; set; }

        // -------------------------
        // Soft delete
        // -------------------------
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // -------------------------
        // Notes / audit
        // -------------------------
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------------------
        // Navigation collections
        // -------------------------
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();

        // TODO: Create Invoice entity
        // public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        
        public ICollection<EquipmentRecall> Recalls { get; set; } = new List<EquipmentRecall>();

        public ICollection<EquipmentDocument> EquipmentDocuments { get; set; } = new List<EquipmentDocument>();
    }
}
