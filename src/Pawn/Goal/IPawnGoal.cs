using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Controller;
namespace Pawn.Goal {
	public interface IPawnGoal
	{
		ITask GetTask(PawnController pawnController);
	}
}
