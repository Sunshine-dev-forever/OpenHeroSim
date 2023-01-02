using Godot;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn {
	public class Item
	{
		public double Healing {get;}
		public Spatial Mesh {get;}
		public Item(double _Healing, Spatial mesh) {
			Healing = _Healing;
			Mesh = mesh;
		}
	}
}
