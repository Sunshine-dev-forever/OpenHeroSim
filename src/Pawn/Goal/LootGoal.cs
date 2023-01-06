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
			return new TargetInteractableTask(action, nearbyLoot[0]);
		}

		private void processItem(IItem item, PawnController pawnController, ItemContainer container) {
			if(item is Equipment) {
				Equipment newWeapon = (Equipment) item;
				Equipment? currentWeapon = pawnController.PawnInventory.GetWornEquipment(EquipmentType.HELD);
				if(currentWeapon == null || (newWeapon.Damage > currentWeapon.Damage)) {
					//if we want the weapon, we take the weapon
					pawnController.PawnInventory.WearEquipment(newWeapon);
					container.Items.Remove(newWeapon);
				} 
			} else if (item is Consumable) {
				pawnController.PawnInventory.inventory.Add((Consumable) item);
				container.Items.Remove(item);
			} else {
				Log.Error("Loot goal mangaged to find something other than a weapon or consumable as an item");
			}
		}
	}
}
