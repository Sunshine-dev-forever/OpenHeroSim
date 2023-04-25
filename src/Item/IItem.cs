using Godot;
using System;
using Serilog;
using Pawn;

namespace Item {
	public interface IItem {
		public Node3D Mesh {get;}
		public void QueueFree();
	}
}