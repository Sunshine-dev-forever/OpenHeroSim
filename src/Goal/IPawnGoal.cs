using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;

public interface IPawnGoal
{
	ITask GetTask(PawnController pawnController);
}
