using Interactable;
using Pawn.Components;
using System.Collections.Generic;

namespace Pawn;

public class PawnSenses {
    const int MAX_PAWNS_TO_SEE = 20;
    const int VISION_RANGE = 10;
    readonly KdTreeController kdTreeController;
    readonly IPawnController pawnController;

    public PawnSenses(KdTreeController _kdTreeController, IPawnController _pawnController) {
        kdTreeController = _kdTreeController;
        pawnController = _pawnController;
    }

    public SensesStruct UpdatePawnSenses(SensesStruct sensesStruct) {
        // nearby pawns will not include the current pawn
        List<IInteractable> visableInteractables =
            kdTreeController.GetNearestInteractableToInteractable(pawnController, MAX_PAWNS_TO_SEE)
                .FindAll((IInteractable interactable) => 
                    interactable.GlobalTransform.Origin.DistanceTo(pawnController.GlobalTransform.Origin) < VISION_RANGE);

        // TODO: should be able to use .select instead of ConvertAll in the future
        sensesStruct.nearbyPawns = visableInteractables
            .FindAll(interactable => interactable is IPawnController)
            .ConvertAll(interactable => (IPawnController)interactable);

        // will also be able to find all object containers this way
        sensesStruct.nearbyContainers = visableInteractables
            .FindAll(interactable => interactable is ItemContainer)
            .ConvertAll(interactable => (ItemContainer)interactable);

        sensesStruct.nearbyShops = visableInteractables
            .FindAll(interactable => interactable is Shop)
            .ConvertAll(interactable => (Shop)interactable);

        // passing a struct through a function will cause it to be copied, so I have to return the new struct
        return sensesStruct;
    }
}
