using Interactable;
using System.Collections.Generic;

namespace Pawn.Components;
public struct SensesStruct
{
    public SensesStruct()
    {
        nearbyPawns = new List<IPawnController>();
        nearbyContainers = new List<ItemContainer>();
    }

    public List<ItemContainer> nearbyContainers;
    public List<IPawnController> nearbyPawns;
}