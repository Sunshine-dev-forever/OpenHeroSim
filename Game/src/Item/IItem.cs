using Godot;
using System;
using Serilog;
using Pawn;

// represents an item,or anything that will/could be put in a chest
namespace Item {
	public interface IItem {
		public string Name {get;}
		public void QueueFree();
	}
}