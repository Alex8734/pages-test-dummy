using HerbalGarden.Core;
using Microsoft.AspNetCore.Components;

namespace HerbalGarden.Pages;

public sealed partial class Greenhouse : IDisposable
{
    private const int NumberOfPots = 48;
    private const string On = "on";
    private const int SecondsPerGrowthDay = 8;

    private static bool _isPlanting = true;
    private static HerbType _selectedHerb = HerbType.Basil;
    private static string _message = string.Empty;
    private static DateTime _messageSetTime = DateTime.MinValue;

    // we don't know collections or interfaces yet
    private static readonly Pot[] pots = CreatePots();

    private Timer? _timer = null;
    private DateTime? _lastGrowth = null;
    private int _secondsToNextGrowth = SecondsPerGrowthDay;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _lastGrowth = DateTime.Now;
        _timer = new Timer(_ => TimerTick(), null, TimeSpan.Zero, TimeSpan.FromSeconds(0.5D));
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private void TimerTick()
    {
        if (_message != string.Empty
            && DateTime.Now - _messageSetTime > TimeSpan.FromSeconds(2))
        {
            _message = string.Empty;
        }

        if (_lastGrowth is not null)
        {
            var timeSinceLastGrowth = DateTime.Now - _lastGrowth.Value;
            var secondsSinceStart = (int) Math.Floor(timeSinceLastGrowth.TotalSeconds);
            _secondsToNextGrowth = Math.Max(SecondsPerGrowthDay - secondsSinceStart, 0);
            if (secondsSinceStart >= SecondsPerGrowthDay)
            {
                Calendar.Advance();
                Grow(Calendar.CurrentDate);
                _lastGrowth = DateTime.Now;
            }
        }

        StateHasChanged();
    }

    public void Grow(DateOnly onDate)
    {
        for (var i = 0; i < pots.Length; i++)
        {
            var pot = pots[i];
            pot.Herb?.Grow(onDate);
        }
    }

    private static Pot[] CreatePots()
    {
        var newPots = new Pot[NumberOfPots];
        for (var i = 0; i < newPots.Length; i++)
        {
            newPots[i] = new Pot();
        }

        return newPots;
    }

    private void HandleOperationModeChanged(ChangeEventArgs changeEvent, bool isPlanting)
    {
        if (changeEvent.Value is not On)
        {
            return;
        }

        _isPlanting = isPlanting;
    }

    private void HandleHerbSelectionChanged(ChangeEventArgs changeEvent, HerbType type)
    {
        if (changeEvent.Value is not On)
        {
            return;
        }

        _selectedHerb = type;
    }

    private void HandlePotClicked(Pot pot)
    {
        if (_isPlanting)
        {
            bool plantSuccess = pot.Plant(_selectedHerb);
            if (!plantSuccess)
            {
                ShowMessage("Pot is not empty.");
            }
        }
        else
        {
            bool harvestSuccess = pot.TryHarvest(out var herb);
            if (!harvestSuccess || herb is null)
            {
                ShowMessage("Pot is empty.");
            }
            else
            {
                var completedOrder = OrderManagement.ProcessHarvestedHerb(herb);
                if (completedOrder is not null)
                {
                    ShowMessage($"Order {completedOrder.OrderNo} completed!");
                }
            }
        }
    }

    private static void ShowMessage(string message)
    {
        _message = message;
        _messageSetTime = DateTime.Now;
    }
}
