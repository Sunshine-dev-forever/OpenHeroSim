using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
namespace Pawn.Goal {
	public class LootGoal : IPawnGoal
	{
		public ITask GetTask(PawnController pawnController, SensesStruct sensesStruct) {
			List<ItemContainer> nearbyLoot = sensesStruct.nearbyContainers;
			if(nearbyLoot.Count > 0) {
				//we can loot something!
				Log.Information("A lootable is nearby!");
			}
			return new InvalidTask();
		}
	}
}
