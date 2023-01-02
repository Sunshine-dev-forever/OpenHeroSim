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
			return new StaticPointTask(new WaitAction(pawnController, 10000),  1.5f, pawnController.GlobalTransform.origin);
		}
	}
}
