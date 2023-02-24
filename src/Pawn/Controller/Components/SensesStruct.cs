using System.Collections.Generic;
using Pawn.Controller;

namespace Pawn.Controller.Components {
	public struct SensesStruct {
		public SensesStruct() {
			nearbyPawns = new List<PawnController>();
			nearbyContainers = new List<ItemContainer>();
		}

		public List<ItemContainer> nearbyContainers;
		public List<PawnController> nearbyPawns;
	}
}