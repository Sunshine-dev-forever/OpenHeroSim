using Godot;
using System;
using Serilog;
using Pawn;
namespace Item {
	//Consumable is a single-use item that a pawn can use to heal itself
	//In the future Consumables will have many possible effects, 
	//but for now it is just healing
	public class Consumable : IItem
	{
		public double Healing {get;}
		public Node3D Mesh {get;}
		public Consumable(double _Healing, Node3D mesh) {
			Healing = _Healing;
			Mesh = mesh;
		}

		public void QueueFree(){
			Mesh.QueueFree();
		}
	}
}
