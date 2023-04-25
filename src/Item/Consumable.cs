using Godot;
using System;
using Serilog;
using Pawn;
namespace Item {
	public partial class Consumable : IItem
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
