using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
namespace Pawn.Goal {
	public class WanderGoal : IPawnGoal
	{
		public ITask GetTask(PawnController pawnController, SensesStruct sensesStruct) {
			int sideLength = 50;
			Random random = new Random();
			float x = (float) ((random.NextDouble() * sideLength) - (sideLength/2));
			float z = (float) ((random.NextDouble() * sideLength) - (sideLength/2));
			int waitTimeMilliseconds = 2000;
			IAction action = ActionBuilder.Start(pawnController, () => {})
										.Animation(AnimationName.Idle)
										.AnimationPlayLength(waitTimeMilliseconds)
										.Finish();
			return new StaticPointTask(action,  1.5f, new Godot.Vector3(x,5,z));
		}
	}
}
