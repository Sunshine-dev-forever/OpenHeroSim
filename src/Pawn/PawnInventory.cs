using System.Collections.Generic;
using Godot;
using System;
using Serilog;
using Pawn.Controller;
using Pawn.Item;
namespace Pawn {
	public class PawnInventory
	{
		//for now all pawns can only carry 5 items
		private static int INVENTORY_SPACE = 5;
		//Held items should be seperate, but that is a later issue
		private Dictionary<EquipmentType, Equipment> wornGear;
		private List<IItem> bag;
		public PawnInventory() {
			bag = new List<IItem>();
			wornGear = new Dictionary<EquipmentType, Equipment>();
		}

		//Returns all items in the pawn's inventory, including equiptment
		public List<IItem> GetAllItems() {
			List<IItem> rtn = new List<IItem>();
			rtn.AddRange(bag);
			rtn.AddRange(wornGear.Values);
			return rtn;
		}

		public List<IItem> GetAllItemsInBag() {
			return new List<IItem>(bag);
		}

		public void AddItem(IItem item) {
			bag.Add(item);
		}

		//Tries to remove items from the bag first
		//Returns true if the item was successfully removed, otherwise false
		public bool RemoveItem(IItem item) {
			//Try to remove items from bag first
			if(bag.Contains(item)) {
				return bag.Remove(item);
			} else if (item is Equipment) {
				//Then we will remove equipt gear
				Equipment equipment = (Equipment) item;
				if(wornGear.ContainsValue(equipment)){
					return wornGear.Remove(equipment.EquipmentType);
				}
			}
			//failed to remove anything
			return false;
		}

		//Returns all items in the pawn's inventory, including equiptment
		//Removes the reference of all items from the pawns inventory
		public List<IItem> EmptyAllItems() {
			List<IItem> rtn = GetAllItems();
			bag = new List<IItem>();
			wornGear = new Dictionary<EquipmentType, Equipment>();
			return rtn;
		}

		//returns null if no equiptment of that type is worn
		public Equipment? GetWornEquipment(EquipmentType type) {
			if(wornGear.ContainsKey(type)) {
				return wornGear[type];
			} else {
				return null;
			}
		}

		public void WearEquipment(Equipment equipment) {
			//if we already were wearing something
			if(wornGear.ContainsKey(equipment.EquipmentType)) {
				Equipment oldEquipment = wornGear[equipment.EquipmentType];
				//store it for later
				bag.Add(oldEquipment);
			}
			wornGear[equipment.EquipmentType] = equipment;
		}

		//TODO: somehow combine GetTotalEquiptmentDefense and GetTotalEquiptmentDamage and GetTotal<whateverElse> into one function
		public double GetTotalEquiptmentDefense() {
			double total = 0;
			foreach(Equipment equipment in wornGear.Values) {
				total += equipment.Defense;
			}
			return total;
		}

		public double GetTotalEquiptmentDamage() {
			double total = 0;
			foreach(Equipment equipment in wornGear.Values) {
				total += equipment.Damage;
			}
			return total;
		}

		public void QueueFree() {
			foreach(Equipment equipment in wornGear.Values) {
				equipment.QueueFree();
			}
			foreach(IItem item in bag) {
				item.QueueFree();
			}
		}
	}
}
