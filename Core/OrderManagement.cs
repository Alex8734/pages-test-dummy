namespace HerbalGarden.Core;

public static class OrderManagement
{
    private const int MaxOrders = 6;
    private static int _nextOrderNo = 1;
    private static int _activeOrders = 0;
    private static readonly Order?[] orders = new Order?[MaxOrders];
    public static bool CanAddMoreOrders => _activeOrders < MaxOrders;

    public static bool AddOrder(HerbType type, int quantity)
    {
        if (_activeOrders >= MaxOrders)
        {
            return false;
        }

        var order = new Order(_nextOrderNo++, type, quantity, Calendar.CurrentDate);
        int freeSlotIndex = FindFreeSlot();
        orders[freeSlotIndex] = order;
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

        var order = orders[oldestMatchingOrderIndex.Value];
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
        orders[oldestMatchingOrderIndex.Value] = null;

        return order;
    }

    private static int? FindIndexOfOldestOrderOfType(HerbType type)
    {
        Order? oldestOrder = null;
        int? oldestIndex = null;
        for (var i = 0; i < orders.Length; i++)
        {
            var order = orders[i];
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
        for (var i = 0; i < orders.Length; i++)
        {
            if (orders[i] is null)
            {
                return i;
            }
        }

        return -1;
    }
}
