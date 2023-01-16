using System.Collections.Generic;
using Godot;
using System;
using Serilog;
using Pawn.Controller;
using Pawn.Item;
namespace Pawn {

	public class PawnInformation
	{
		public const String NO_FACTION = "none";
		//TODO: any way to get around using strings for factions?
		public string Faction {get; set;} = NO_FACTION;
		public string Name {get; set;} = "Testy McTesterson";
		public double BaseDamage {get; set;} = 10;
		public double Health {get; set;} = 100;
		public double MaxHealth {get; set;} = 100;		
	}
}
