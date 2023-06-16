using System.Reflection.Metadata;
using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn;
using Item;
using Pawn.Targeting;
using Pawn.Components;
using Interactable;

namespace Pawn.Goal {
	public class LootGoal : IPawnGoal
	{
		public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct) {
			ItemContainer? nearbyLoot = GetFirstNonemptyContainer(sensesStruct.nearbyContainers);
			if(nearbyLoot == null) {
				return new InvalidTask();
			}

			System.Action executable = () => {
				for (int i = nearbyLoot.Items.Count - 1 ; i >= 0; i--) {
					IItem item = nearbyLoot.Items[i];
					processItem(item, pawnController, nearbyLoot);
				}
			};

			IAction action = ActionBuilder.Start(pawnController, executable).Animation(AnimationName.Interact).Finish();
			ITargeting targeting = new InteractableTargeting(nearbyLoot);
			return new Task(targeting, action);
		}

		private void processItem(IItem item, IPawnController pawnController, ItemContainer container) {
			container.Items.Remove(item);
			if(item is Equipment) {
				Equipment newWeapon = (Equipment) item;
				Equipment? currentWeapon = pawnController.PawnInventory.GetWornEquipment(EquipmentType.HELD);
				if(currentWeapon == null || (newWeapon.Damage > currentWeapon.Damage)) {
					//if we want the weapon, we take the weapon
					pawnController.PawnInventory.WearEquipment(newWeapon);
				}
			} else if (item is Consumable) {
				bool wasAddSuccessfull = pawnController.PawnInventory.AddItem((Consumable) item);
				if(!wasAddSuccessfull) {
					//I guess the bad is full
					//we delete the item
					item.QueueFree();
				}
			} else {
				Log.Error("Loot goal mangaged to find something other than a weapon or consumable as an item");
			}
		}

		private ItemContainer? GetFirstNonemptyContainer(List<ItemContainer> nearbyContainers) {
			foreach( ItemContainer itemContainer in nearbyContainers) {
				if(itemContainer.Items.Count >= 1) {
					return itemContainer;
				}
			}
			return null;
		}
	}
}
