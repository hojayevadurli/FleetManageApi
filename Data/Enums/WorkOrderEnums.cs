namespace FleetManage.Api.Data.Enums
{
    public enum WorkOrderStatus
    {
        Draft = 0,
        Open = 1,
        InProcess = 2,
        Completed = 3,
        Closed = 4,
        Cancelled = 5,
        Paid = 6
    }

    public enum WorkOrderPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }

    public enum WorkOrderCostSource
    {
        Estimated = 0,
        Manual = 1,
        Invoiced = 2
    }
}
