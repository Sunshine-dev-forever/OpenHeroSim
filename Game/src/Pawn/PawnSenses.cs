using Interactable;
using Pawn.Components;
using System.Collections.Generic;

namespace Pawn;

public class PawnSenses
{
    const int MAX_INTERACTABLES_TO_SEE = 20;
    const int VISION_RANGE = 10;
    readonly KdTreeController kdTreeController;
    readonly IPawnController pawnController;

    public PawnSenses(KdTreeController _kdTreeController, IPawnController _pawnController)
    {
        kdTreeController = _kdTreeController;
        pawnController = _pawnController;
    }

    public SensesStruct UpdatePawnSenses(SensesStruct sensesStruct)
    {
        // nearby pawns will not include the current pawn
        List<IInteractable> visableInteractables =
            kdTreeController.GetNearestInteractableToInteractable(pawnController, MAX_INTERACTABLES_TO_SEE)
                .FindAll((IInteractable interactable) =>
                {
                    return interactable.GlobalTransform.Origin.DistanceTo(
                        pawnController.GlobalTransform.Origin) < VISION_RANGE;
                });

        // TODO: should be able to use .select instead of ConvertAll in the future

        //TODO: instead of haveing 3 (or more in the future) blocks of code like this, I should have a function that filters nearby interactables
        //and caches the result
        sensesStruct.nearbyPawns = visableInteractables
            .FindAll(interactable => { return interactable is IPawnController; })
            .ConvertAll(interactable => { return (IPawnController)interactable; });

        sensesStruct.nearbyContainers = visableInteractables
            .FindAll(interactable => { return interactable is ItemContainer; })
            .ConvertAll(interactable => { return (ItemContainer)interactable; });

        sensesStruct.nearbyBuildings = visableInteractables
             .FindAll(interactable => { return interactable is Building; })
             .ConvertAll(interactable => { return (Building)interactable; });

        // passing a struct through a function will cause it to be copied, so I have to return the new struct
        return sensesStruct;
    }
}
