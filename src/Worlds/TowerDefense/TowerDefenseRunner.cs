using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using System.Threading.Tasks;
using Pawn;
using Pawn.Controller;
using Pawn.Goal;
using Pawn.Item;
using Pawn.Action.Ability;
using Util;

namespace Worlds.TowerDefense {
	public class TowerDefenseRunner : Node
	{
		private List<PawnController> pawns = new List<PawnController>();
		private KdTreeController kdTreeController = null!; 
		public override void _Ready()
		{
			kdTreeController = new KdTreeController();
			this.AddChild(kdTreeController);
		}
		
		public override void _Input(InputEvent input) {
			if(input.IsActionPressed("mouse_left_click")) {
				//Do nothing... for now
			} else if(input.IsActionPressed("ui_left")) {

			} else if (input.IsActionPressed("ui_right")) {

			} else if(input.IsActionPressed("ui_up")) {

			} else if(input.IsActionPressed("ui_down")){

			}
		}

		public override void _Process(float  delta){

		}
	}

}
