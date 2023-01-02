using System.Collections.Generic;

namespace Pawn.Controller
{
	public class SensesController
	{
		private const int MAX_PAWNS_TO_SEE = 20;
		private const int VISION_RANGE = 10;
		private KdTreeController kdTreeController;

		private PawnController pawnController;

		public SensesController(KdTreeController _kdTreeController, PawnController _pawnController)
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
									return interactable.GlobalTransform.origin.DistanceTo(pawnController.GlobalTransform.origin) < VISION_RANGE;
								});
			//TODO: should be able to use .select instead of ConvertAll in the future
			sensesStruct.nearbyPawns = visableInteractables
										.FindAll( (interactable) => {return interactable is PawnController;})
										.ConvertAll<PawnController>( (interactable) => {return (PawnController) interactable;});
			//will also be able to find all object containers this way
			//TODO: sensesStruct.nearbyObject = visibleInteractables.....
			//passing a struct through a function will cause it to be copied, so I have to return the new struct
			return sensesStruct;
		}
	}
}