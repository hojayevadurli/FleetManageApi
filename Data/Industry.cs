using System.ComponentModel.DataAnnotations;

namespace FleetManage.Api.Data
{
    public class Industry
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = default!;
    }
}
