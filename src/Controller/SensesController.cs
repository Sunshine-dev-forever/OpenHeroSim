using System.Collections.Generic;

public class SensesController
{
	private const int MAX_PAWNS_TO_SEE = 20;
	private const int VISION_RANGE = 10;
	private KdTreeController kdTreeController;

	private PawnController pawnController;

	public SensesController(KdTreeController _kdTreeController, PawnController _pawnController) {
		kdTreeController = _kdTreeController;
		pawnController = _pawnController;
	}

	public SensesStruct UpdatePawnSenses(SensesStruct sensesStruct) {
		//nearby pawns will not include the current pawn
		List<PawnController> nearbyPawns = kdTreeController.GetNearestPawnsToPawn(pawnController, MAX_PAWNS_TO_SEE);
		nearbyPawns = nearbyPawns.FindAll( (PawnController otherPawnController) => {
			return otherPawnController.GlobalTransform.origin.DistanceTo(pawnController.GlobalTransform.origin) < VISION_RANGE;
		});
		sensesStruct.nearbyPawns = nearbyPawns;
		//passing a struct through a function will cause it to be copied, so I have to return the new struct
		return sensesStruct;
	}
}