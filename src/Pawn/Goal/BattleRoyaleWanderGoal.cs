using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
using Testing.BattleRoyale;
namespace Pawn.Goal {
	public class BattleRoyaleWanderGoal : IPawnGoal
	{
		public BattleRoyaleWanderGoal() {}

		public ITask GetTask(PawnController pawnController, SensesStruct sensesStruct) {
			int sideLength = (int) (FogController.GetFogController().GetFogPosition() * 2);
			Random random = new Random();
			float x = (float) ((random.NextDouble() * sideLength) - (sideLength/2));
			float z = (float) ((random.NextDouble() * sideLength) - (sideLength/2));
			int waitTimeMilliseconds = 2000;
			IAction action = ActionBuilder.Start(pawnController, () => {})
										.Animation(AnimationName.Idle)
										.AnimationPlayLength(waitTimeMilliseconds)
										.Finish();
			return new StaticPointTask(action, new Godot.Vector3(x,5,z));
		}
	}
}
