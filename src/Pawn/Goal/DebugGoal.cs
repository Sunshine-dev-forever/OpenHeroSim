using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
namespace Pawn.Goal {
	public class DebugGoal : IPawnGoal
	{
		public ITask GetTask(PawnController pawnController) {
			float x = pawnController.GlobalTransform.origin.x;
			float z = pawnController.GlobalTransform.origin.z;
			return new StaticPointTask(new WaitAction(pawnController, 10000),  1.5f, new Godot.Vector3(x,5,z));
		}
	}
}
