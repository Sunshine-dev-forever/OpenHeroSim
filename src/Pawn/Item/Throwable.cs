using Godot;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn.Item {
	public class Throwable : IItem
	{
		public double Damage {get; set;} = 0;
		public Spatial Mesh {get;}
		public Throwable(Spatial mesh, double _damage) {
			Mesh = mesh;
			Damage = _damage;
		}
		public void QueueFree() {
			Mesh.QueueFree();
		}
	}
}
