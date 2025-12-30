using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FleetManage.Api.Data
{
  
    public class Tenant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    }

    

    public abstract class TenantEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
    }

    // ---- New Base Class ----
    public abstract class Equipment : TenantEntity
    {
        public string Number { get; set; } = default!;
        public string Vin { get; set; } = default!;
        public int? Year { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public DateTime? PurchasedAt { get; set; }
    }

    // ---- Derived Classes ----
    public class Truck : Equipment
    {
        public string? PlateNumber { get; set; }
        public double? Mileage { get; set; }
        public string? EngineType { get; set; }
        public string? Status { get; set; } // active, in shop, etc.

        // Navigation: list of work orders, etc.
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }

    public class Trailer : Equipment
    {
        public string? Type { get; set; } 
        public double? Length { get; set; }
        public double? WeightCapacity { get; set; }
        public string? Status { get; set; }
    }

    // ---- Example ----
    //public class WorkOrder : TenantEntity
    //{
    //    public Guid TruckId { get; set; }
    //    public Truck Truck { get; set; } = default!;
    //    public string Title { get; set; } = default!;
    //    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    //    public string? Notes { get; set; }
    //}


}
