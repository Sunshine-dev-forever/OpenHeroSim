using Godot;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn {
	public class Weapon
	{
		public double Damage {get;}
		public Spatial Mesh {get;}
		public Weapon(double damageStat, Spatial mesh) {
			Damage = damageStat;
			Mesh = mesh;
		}
		public void Delete() {
			Mesh.QueueFree();
		}
	}
}
