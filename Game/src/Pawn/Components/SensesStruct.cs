using Interactable;
using System.Collections.Generic;

namespace Pawn.Components;

public struct SensesStruct
{
    public SensesStruct()
    {
        nearbyPawns = new List<IPawnController>();
        nearbyContainers = new List<ItemContainer>();
        nearbyShops = new List<Shop>();
    }

    public List<ItemContainer> nearbyContainers;
    public List<IPawnController> nearbyPawns;
    public List<Shop> nearbyShops;
}
