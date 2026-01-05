using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManage.Api.Data
{
    public class EquipmentType
    {
        public int Id { get; set; }

        public int IndustryId { get; set; }
        public Industry Industry { get; set; } = default!;

        public int FleetCategoryId { get; set; }
        public FleetCategory FleetCategory { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;
        
        public string? Code { get; set; }
        
        // 1=Odometer, 2=Hours, 3=Both
        public int MeterMode { get; set; } 

        public bool HasVin { get; set; }
        public bool HasSerial { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
