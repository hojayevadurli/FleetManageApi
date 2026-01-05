using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManage.Api.Data
{
    public class Department
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] // User provided specific IDs (101, 201 etc)
        public int Id { get; set; }

        public int IndustryId { get; set; }
        public Industry Industry { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;

        public string? Code { get; set; } // e.g., "OPS", "MNT"
        
        public bool IsActive { get; set; } = true;
        
        public bool ListEquipment { get; set; } = false;
    }
}
