namespace HerbalGarden.Core;

public sealed class Herb(HerbType type, DateOnly plantedOn)
{
    public HerbType Type { get; } = type;
    public DateOnly PlantedOn { get; } = plantedOn;
    public GrowthState GrowthState { get; private set; } = GrowthState.Sapling;

    public void Grow(DateOnly currentDate)
    {
        int growthDays = GetDaysBetween(PlantedOn, currentDate);
        GrowthState = GetStateFromGrowthDays(growthDays);
    }

    private GrowthState GetStateFromGrowthDays(int growthDays)
    {
        (int saplingDays, int growingDays, int matureDays) = GetDaysInState();
        if (growthDays <= saplingDays)
        {
            return GrowthState.Sapling;
        }

        growthDays -= saplingDays;

        if (growthDays <= growingDays)
        {
            return GrowthState.Growing;
        }

        growthDays -= growingDays;

        return growthDays <= matureDays
            ? GrowthState.Mature
            : GrowthState.Wilting;
    }

    private GrowthDays GetDaysInState()
    {
        // we don't know collections yet
        return Type switch
               {
                   HerbType.Basil    => new GrowthDays(2, 6, 4),
                   HerbType.Mint     => new GrowthDays(3, 5, 7),
                   HerbType.Rosemary => new GrowthDays(4, 10, 12),
                   HerbType.Thyme    => new GrowthDays(1, 8, 20),
                   // we don't know exceptions yet
                   _ => new GrowthDays(-1, -1, -1)
               };
    }

    private static int GetDaysBetween(DateOnly start, DateOnly end)
    {
        if (start > end)
        {
            (start, end) = (end, start);
        }

        int diff = end.DayNumber - start.DayNumber;

        return Math.Max(1, diff);
    }

    private sealed record GrowthDays(int DaysSapling, int DaysGrowing, int DaysMature);
}
