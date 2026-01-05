using System.ComponentModel.DataAnnotations;

namespace FleetManage.Api.DTOs
{
    public class FleetCategoryDto
    {
        public int Id { get; set; }
        
        public int IndustryId { get; set; }
        public string IndustryName { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Code { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool ListEquipment { get; set; }
    }

    public class CreateFleetCategoryDto
    {
        [Required]
        public int Id { get; set; } // DatabaseGeneratedOption.None

        public int IndustryId { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        public string? Code { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool ListEquipment { get; set; } = false;
    }

    public class UpdateFleetCategoryDto
    {
        public int IndustryId { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        public string? Code { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool ListEquipment { get; set; }
    }
}
