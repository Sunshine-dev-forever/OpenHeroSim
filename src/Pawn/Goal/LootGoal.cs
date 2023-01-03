using System.Reflection.Metadata;
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
			if(nearbyLoot.Count == 0) {
				return new InvalidTask();
				
			}
			IAction action = new LootAction(pawnController, nearbyLoot[0]);
			return new TargetInteractableTask(action, 2, nearbyLoot[0]);
		}
	}
}
