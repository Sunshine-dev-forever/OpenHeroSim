using Godot;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn.Item {
	public class Consumable : IItem
	{
		public double Healing {get;}
		public Spatial Mesh {get;}
		public Consumable(double _Healing, Spatial mesh) {
			Healing = _Healing;
			Mesh = mesh;
		}

		public void QueueFree(){
			Mesh.QueueFree();
		}
	}
}
