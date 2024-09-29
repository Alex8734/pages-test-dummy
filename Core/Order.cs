namespace HerbalGarden.Core;

public sealed class Order(int orderNo, HerbType type, int quantity, DateOnly orderDate)
{
    public int OrderNo { get; } = orderNo;
    public HerbType Type { get; } = type;
    public int QuantityOrdered { get; } = quantity;
    public DateOnly OrderDate { get; } = orderDate;
    public int QuantityFulfilled { get; private set; }
    public bool Completed => QuantityFulfilled >= QuantityOrdered;

    public bool HerbShipped(Herb herb)
    {
        if (QuantityFulfilled >= QuantityOrdered
            || herb.Type != Type
            || herb.GrowthState is GrowthState.Wilting)
        {
            return false;
        }

        QuantityFulfilled++;

        return true;
    }
}
