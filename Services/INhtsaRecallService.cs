namespace FleetManage.Api.Services
{
    public interface INhtsaRecallService
    {
        Task<int> SyncRecallsForEquipmentAsync(Guid equipmentId);
    }
}
