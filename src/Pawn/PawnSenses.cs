using System.Collections.Generic;
using Pawn.Components;
using Interactable;

namespace Pawn
{
	public partial class PawnSenses
	{
		private const int MAX_PAWNS_TO_SEE = 20;
		private const int VISION_RANGE = 10;
		private KdTreeController kdTreeController;

		private PawnController pawnController;

		public PawnSenses(KdTreeController _kdTreeController, PawnController _pawnController)
		{
			kdTreeController = _kdTreeController;
			pawnController = _pawnController;
		}

		public SensesStruct UpdatePawnSenses(SensesStruct sensesStruct)
		{
			//nearby pawns will not include the current pawn
			List<IInteractable> visableInteractables = 
				kdTreeController.GetNearestInteractableToInteractable(pawnController, MAX_PAWNS_TO_SEE)
								.FindAll((IInteractable interactable) =>
								{
									return interactable.GlobalTransform.Origin.DistanceTo(pawnController.GlobalTransform.Origin) < VISION_RANGE;
								});
			//TODO: should be able to use .select instead of ConvertAll in the future
			sensesStruct.nearbyPawns = visableInteractables
										.FindAll( (interactable) => {return interactable is PawnController;})
										.ConvertAll<PawnController>( (interactable) => {return (PawnController) interactable;});
			//will also be able to find all object containers this way
			sensesStruct.nearbyContainers = visableInteractables
										.FindAll( (interactable) => {return interactable is ItemContainer;})
										.ConvertAll<ItemContainer>( (interactable) => {return (ItemContainer) interactable;});
			//passing a struct through a function will cause it to be copied, so I have to return the new struct
			return sensesStruct;
		}
	}
}