using System.Reflection.Metadata;
using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
using Pawn.Item;
namespace Pawn.Goal {
	public class LootGoal : IPawnGoal
	{
		public ITask GetTask(PawnController pawnController, SensesStruct sensesStruct) {
			List<ItemContainer> nearbyLoot = sensesStruct.nearbyContainers;
			if(nearbyLoot.Count == 0) {
				return new InvalidTask();
			}
			ItemContainer containerToLoot = nearbyLoot[0];

			System.Action executable = () => {
				Weapon nextWeapon = (Weapon) containerToLoot.ContainedItem;

				if( (pawnController.Weapon == null) || (nextWeapon.Damage > pawnController.Weapon.Damage) ){
					//changing things in a multithreaded environment!
					//awesome
					pawnController.SetWeapon(nextWeapon);
					containerToLoot.ContainedItem = null;
					containerToLoot.QueueFree();
				}
			};

			IAction action = ActionBuilder.Start(pawnController, executable).Animation(AnimationName.Interact).Finish();
			return new TargetInteractableTask(action, 2, nearbyLoot[0]);
		}
	}
}
