namespace HerbalGarden.Core;

public sealed class Pot
{
    public Herb? Herb { get; private set; }
    public bool IsEmpty => Herb is null;

    public bool Plant(HerbType type)
    {
        if (!IsEmpty)
        {
            return false;
        }

        Herb = new Herb(type, Calendar.CurrentDate);

        return true;
    }
    
    public bool TryHarvest(out Herb? herb)
    {
        if (IsEmpty)
        {
            herb = null;
            return false;
        }

        herb = Herb;
        Herb = null;

        return true;
    }
}
