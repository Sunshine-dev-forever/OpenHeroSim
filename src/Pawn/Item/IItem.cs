using Godot;
using System;
using Serilog;
using Pawn.Controller;

namespace Pawn.Item {
	public interface IItem {
		public Node3D Mesh {get;}
		public void QueueFree();
	}
}