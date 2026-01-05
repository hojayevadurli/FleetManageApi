using FleetManage.Api.Data.Enums;

namespace FleetManage.Api.DTOs
{
    public class EquipmentDto
    {
        public Guid Id { get; set; }
        public int EquipmentTypeId { get; set; }
        public string EquipmentTypeName { get; set; } = default!; // Flattened
        public string EquipmentTypeCode { get; set; } = default!;

        public int FleetCategoryId { get; set; }
        public string FleetCategoryName { get; set; } = default!;

        public string UnitNumber { get; set; } = default!;
        public string? DisplayName { get; set; }
        public string? Vin { get; set; }
        public string? SerialNumber { get; set; }
        public string? PlateNumber { get; set; }

        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }

        public EquipmentLifecycleStatus LifecycleStatus { get; set; }
        public EquipmentOperationalStatus OperationalStatus { get; set; }

        public int? OdometerCurrent { get; set; }
        public int? HoursCurrent { get; set; }

        public DateOnly? AcquiredDate { get; set; }
        public DateOnly? InServiceDate { get; set; }
        public DateOnly? OutOfServiceDate { get; set; }

        public string? Notes { get; set; }
        
        public List<EquipmentRecallDto> Recalls { get; set; } = new();
    }

    public class CreateEquipmentDto
    {
        public int EquipmentTypeId { get; set; }
        public int FleetCategoryId { get; set; }

        public string UnitNumber { get; set; } = default!;
        public string? DisplayName { get; set; }
        public string? Vin { get; set; }
        public string? SerialNumber { get; set; }
        public string? PlateNumber { get; set; }

        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }

        public EquipmentLifecycleStatus LifecycleStatus { get; set; }
        public EquipmentOperationalStatus OperationalStatus { get; set; }

        public int? OdometerCurrent { get; set; }
        public int? HoursCurrent { get; set; }

        public DateOnly? AcquiredDate { get; set; }
        public DateOnly? InServiceDate { get; set; }
        
        public string? Notes { get; set; }
    }

    public class UpdateEquipmentDto
    {
        public int EquipmentTypeId { get; set; }
        public int FleetCategoryId { get; set; }

        public string UnitNumber { get; set; } = default!;
        public string? DisplayName { get; set; }
        public string? Vin { get; set; }
        public string? SerialNumber { get; set; }
        public string? PlateNumber { get; set; }

        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }

        public EquipmentLifecycleStatus LifecycleStatus { get; set; }
        public EquipmentOperationalStatus OperationalStatus { get; set; }

        public int? OdometerCurrent { get; set; }
        public int? HoursCurrent { get; set; }

        public DateOnly? AcquiredDate { get; set; }
        public DateOnly? InServiceDate { get; set; }
        public DateOnly? OutOfServiceDate { get; set; }

        public string? Notes { get; set; }
    }
}
