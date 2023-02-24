using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Controller;
using Pawn.Controller.Components;
namespace Pawn.Goal {
	public interface IPawnGoal
	{
		ITask GetTask(PawnController pawnController, SensesStruct sensesStruct);
	}
}
