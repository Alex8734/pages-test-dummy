namespace HerbalGarden.Core;

public static class OrderManagement
{
    private const int MaxOrders = 6;
    private static int _nextOrderNo = 1;
    private static int _activeOrders = 0;
    public static Order?[] Orders { get; } = new Order?[MaxOrders];
    public static bool CanAddMoreOrders => _activeOrders < MaxOrders;

    public static bool AddOrder(HerbType type, int quantity)
    {
        if (_activeOrders >= MaxOrders)
        {
            return false;
        }

        var order = new Order(_nextOrderNo++, type, quantity, Calendar.CurrentDate);
        int freeSlotIndex = FindFreeSlot();
        Orders[freeSlotIndex] = order;
        _activeOrders++;

        return true;
    }

    public static Order? ProcessHarvestedHerb(Herb herb)
    {
        int? oldestMatchingOrderIndex = FindIndexOfOldestOrderOfType(herb.Type);
        if (oldestMatchingOrderIndex is null)
        {
            return null;
        }

        var order = Orders[oldestMatchingOrderIndex.Value];
        if (order is null)
        {
            return null;
        }

        order.HerbShipped(herb);
        if (!order.Completed)
        {
            return null;
        }

        _activeOrders--;
        Orders[oldestMatchingOrderIndex.Value] = null;

        return order;
    }

    private static int? FindIndexOfOldestOrderOfType(HerbType type)
    {
        Order? oldestOrder = null;
        int? oldestIndex = null;
        for (var i = 0; i < Orders.Length; i++)
        {
            var order = Orders[i];
            if (order is not null && order.Type == type)
            {
                if (oldestOrder is null || order.OrderDate < oldestOrder.OrderDate)
                {
                    oldestOrder = order;
                    oldestIndex = i;
                }
            }
        }

        return oldestIndex;
    }

    private static int FindFreeSlot()
    {
        for (var i = 0; i < Orders.Length; i++)
        {
            if (Orders[i] is null)
            {
                return i;
            }
        }

        return -1;
    }
}
