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
		public Node3D Mesh {get;}
		//determines which 'slot' an equipments occupies
		//for example a pawn can only equipt 1 head-piece at a tume
		public EquipmentType EquipmentType{get; set;}
		public Equipment(Node3D mesh, EquipmentType equipmentType) {
			Mesh = mesh;
			EquipmentType = equipmentType;
		}
		public void QueueFree() {
			Mesh.QueueFree();
		}
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
