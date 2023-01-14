using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
using Pawn.Targeting;
namespace Pawn.Goal {
	public class DebugGoal : IPawnGoal
	{
		public ITask GetTask(PawnController pawnController, SensesStruct sensesStruct) {
			int waitTimeMilliseconds = 10000;
			IAction waitAction = ActionBuilder.Start(pawnController, () => {})
										.Animation(AnimationName.Idle)
										.AnimationPlayLength(waitTimeMilliseconds)
										.Finish();
			//we have the pawn target itself
			ITargeting targeting = new InteractableTargeting(pawnController);
			return new Task(targeting, waitAction);
		}
	}
}
