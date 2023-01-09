using System.Collections.Generic;
using Godot;
using System;
using Serilog;
using Pawn.Controller;
using Pawn.Item;
namespace Pawn {
	public class PawnInventory
	{
		//Held items should be seperate, but that is a later issue
		public Dictionary<EquipmentType, Equipment> wornGear;
		public List<IItem> inventory;
		public PawnInventory() {
			inventory = new List<IItem>();
			wornGear = new Dictionary<EquipmentType, Equipment>();
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
				inventory.Add(oldEquipment);
			}
			wornGear[equipment.EquipmentType] = equipment;
		}

		public void QueueFree() {
			foreach(Equipment equipment in wornGear.Values) {
				equipment.QueueFree();
			}
			foreach(IItem item in inventory) {
				item.QueueFree();
			}
		}
	}
}
