using Godot;
using System;
using Serilog;
using Pawn.Controller;

namespace Pawn.Item {
	public interface IItem {
		public Spatial Mesh {get;}
		public void QueueFree();
	}
}