using System.Threading;
using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
using Pawn.Item;
namespace Pawn.Goal {
	public class HealGoal : IPawnGoal
	{
		public ITask GetTask(PawnController pawnController, SensesStruct sensesStruct) {
			if(pawnController.ItemList.Count == 0 || pawnController.health > 50) {
				return new InvalidTask();
			}

			Consumable potion = pawnController.ItemList[0];
			System.Action executable = () => {
				pawnController.ItemList.Remove(potion);
				//TODO: TakeDamage should be called 'change health'
				pawnController.TakeDamage(potion.Healing * (-1));
			};
			//we know it is only health potions
			IAction action = ActionBuilder.Start(pawnController, executable).Animation(AnimationName.Drink).Finish();
			return new StaticPointTask(action, 1.5f, pawnController.GlobalTransform.origin);
		}
	}
}
