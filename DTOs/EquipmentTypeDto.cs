namespace FleetManage.Api.DTOs
{
    public class EquipmentTypeDto
    {
        public int Id { get; set; }
        public int IndustryId { get; set; }
        public int FleetCategoryId { get; set; }
        public string Name { get; set; } = default!;
        public string? Code { get; set; }
        public int MeterMode { get; set; }
        public bool HasVin { get; set; }
        public bool HasSerial { get; set; }
        public bool IsActive { get; set; }
    }
}
