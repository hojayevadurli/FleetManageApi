namespace FleetManage.Api.Data.Enums
{
    public enum EquipmentLifecycleStatus : short
    {
        Active = 1,
        Retired = 2,
        Sold = 3
    }

    public enum EquipmentOperationalStatus : short
    {
        Available = 1,
        InShop = 2,
        OutOfService = 3,
        OnRoad = 4
    }
}
