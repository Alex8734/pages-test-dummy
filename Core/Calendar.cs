namespace HerbalGarden.Core;

public static class Calendar
{
    public static DateOnly CurrentDate { get; private set; } = DateOnly.FromDateTime(DateTime.Now);
    
    public static void Advance()
    {
        CurrentDate = CurrentDate.AddDays(1);
    }
}
