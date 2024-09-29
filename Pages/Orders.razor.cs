using HerbalGarden.Core;

namespace HerbalGarden.Pages;

public sealed partial class Orders
{
    private static readonly string defaultHerbType = HerbType.Basil.ToString();
    private int _amount;
    private string _herbalType = defaultHerbType;
    private HerbType SelectedHerb => Enum.Parse<HerbType>(_herbalType);

    private void AddOrder()
    { 
        if (OrderManagement.AddOrder(SelectedHerb, _amount))
        {
            _amount = 0;
            _herbalType = defaultHerbType;
        }
        else
        {
            // not good for UX, but shows that we can log to console
            Console.WriteLine("Cannot add order");
        }
    }
}

