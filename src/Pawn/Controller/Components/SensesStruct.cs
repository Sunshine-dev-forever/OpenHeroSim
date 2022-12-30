using System.Collections.Generic;
using Pawn.Controller;
public struct SensesStruct {
	public SensesStruct() {
		nearbyPawns = new List<PawnController>();
	}

	public List<PawnController> nearbyPawns;
}