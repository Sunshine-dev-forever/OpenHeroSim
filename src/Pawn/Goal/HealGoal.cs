using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
namespace Pawn.Goal {
	public class HealGoal : IPawnGoal
	{
		public ITask GetTask(PawnController pawnController, SensesStruct sensesStruct) {
			//we know it is only health potions
			if(pawnController.ItemList.Count > 0 && pawnController.health < 90) {
				IAction action = new DrinkPotionAction(pawnController, pawnController.ItemList[0]);
				return new StaticPointTask(action, 1.5f, pawnController.GlobalTransform.origin);
			}
			return new InvalidTask();
		}
	}
}
