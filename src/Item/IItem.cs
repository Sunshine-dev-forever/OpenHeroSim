using Godot;
using System;
using Serilog;
using Pawn;

// represents an item,or anything that will/could be put in a chest
namespace Item {
	public interface IItem {
		public Node3D Mesh {get;}
		public void QueueFree();
		public string Name {get;}
	}
}