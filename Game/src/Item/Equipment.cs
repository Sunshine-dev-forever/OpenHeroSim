using Godot;
using System;
using Serilog;
using Pawn;
namespace Item {
	//represents an item that can be worn or held
	public class Equipment : IItem
	{
		public double Damage {get; set;} = 0;
		public double Defense {get; set;} = 0;
		
		//determines which 'slot' an equipments occupies
		//for example a pawn can only equipt 1 head-piece at a tume
		public EquipmentType EquipmentType{get; set;}

		public string Name {get; private set;} = "equipment";

		public Equipment(EquipmentType equipmentType, string name) {
			Name = name;
			EquipmentType = equipmentType;
		}

		public void QueueFree() {}
	}

	public enum EquipmentType {
		HEAD, 
		CHEST, 
		LEGS, 
		FEET, 
		HANDS, 
		HELD
		}
}
