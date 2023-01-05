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
				for (int i = containerToLoot.Items.Count - 1 ; i >= 0; i--) {
					IItem item = containerToLoot.Items[i];
					processItem(item, pawnController, containerToLoot);
				}
				//TODO: the container should free itself when it is empty after a set time
				containerToLoot.QueueFree();
			};

			IAction action = ActionBuilder.Start(pawnController, executable).Animation(AnimationName.Interact).Finish();
			return new TargetInteractableTask(action, 2, nearbyLoot[0]);
		}

		private void processItem(IItem item, PawnController pawnController, ItemContainer container) {
			if(item is Weapon) {
				Weapon weapon = (Weapon) item;
				if(pawnController.Weapon == null || (weapon.Damage > pawnController.Weapon.Damage)) {
					//if we want the weapon, we take the weapon
					pawnController.SetWeapon(weapon);
					container.Items.Remove(weapon);
				} 
			} else if (item is Consumable) {
				pawnController.ItemList.Add((Consumable) item);
				container.Items.Remove(item);
			} else {
				Log.Error("Loot goal mangaged to find something other than a weapon or consumable as an item");
			}
		}
	}
}
