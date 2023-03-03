using Godot;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn.Item {
	public class Throwable : IItem
	{
		public double Damage {get; set;} = 40;
		//Count starts at 4
		//Oh boy I love arbitrary constants!
		public int Count = 4;
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
