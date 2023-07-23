using System.Threading;
using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn;
using Item;
using Pawn.Targeting;
using Pawn.Components;
namespace Pawn.Goal {
	public class HealGoal : IPawnGoal
	{
		public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct) {
			IItem? currentItem = null;
			foreach( IItem item in pawnController.PawnInventory.GetAllItemsInBag()) {
				if(item is Consumable) {
					currentItem = item;
					break;
				}
			}

			if(currentItem == null) {
				//if we have no consumables, then we early exit
				return new InvalidTask();
			}
			if(pawnController.PawnInformation.Health > 50) {
				//we are not hurt, no reason to use a potion
				return new InvalidTask();
			}

			Consumable potion = (Consumable) currentItem;
			System.Action executable = () => {
				pawnController.PawnInventory.RemoveItem(potion);
				//TODO: TakeDamage should be called 'change health'
				pawnController.TakeHealing(potion.Healing);
			};
			//we know it is only health potions
			IAction action = ActionBuilder.Start(pawnController, executable)
										.Animation(AnimationName.Consume)
										.Finish();
			ITargeting targeting = new InteractableTargeting(pawnController);
			return new Task(targeting, action);
		}
	}
}
