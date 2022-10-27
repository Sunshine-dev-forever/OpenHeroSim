using System.Collections.Generic;

public struct SensesStruct {
	public SensesStruct() {
		nearbyPawns = new List<PawnController>();
	}

	public List<PawnController> nearbyPawns;
}