using Godot;
using System;
using Serilog;
using Pawn;
namespace Item {
	//Consumable is a single-use item that a pawn can use to heal itself
	//In the future Consumables will have many possible effects, 
	//but for now it is just healing
	public class Consumable : IItem
	{
		public double Healing {get;}
		public string Name {get; private set;} = "Consumable";
		public Consumable(double _Healing, string name) {
			Name = name;
			Healing = _Healing;
		}
		public void QueueFree() {}
	}
}
