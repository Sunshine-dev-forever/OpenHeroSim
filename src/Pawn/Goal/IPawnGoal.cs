using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn;
using Pawn.Components;
namespace Pawn.Goal {
	public interface IPawnGoal
	{
		ITask GetTask(PawnController pawnController, SensesStruct sensesStruct);
	}
}
