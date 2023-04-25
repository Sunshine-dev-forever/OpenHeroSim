using Godot;
using System;
using Serilog;
using Pawn;
namespace Item {
	public partial class Throwable : IItem
	{
		public double Damage {get; set;} = 40;
		//Count starts at 4
		//Oh boy I love arbitrary constants!
		public int Count = 4;
		public Node3D Mesh {get;}
		public Throwable(Node3D mesh, double _damage) {
			Mesh = mesh;
			Damage = _damage;
		}
		public void QueueFree() {
			Mesh.QueueFree();
		}
	}
}
