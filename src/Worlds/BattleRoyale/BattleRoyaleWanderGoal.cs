using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn;
using Worlds.BattleRoyale;
using Pawn.Targeting;
using Godot;
using Pawn.Goal;
using Pawn.Components;

namespace Worlds.BattleRoyale {
	public class BattleRoyaleWanderGoal : IPawnGoal
	{
		public BattleRoyaleWanderGoal() {}

		public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct) {
			int sideLength = (int) (FogController.GetFogController().GetFogPosition() * 2);
			Random random = new Random();
			float x = (float) ((random.NextDouble() * sideLength) - (sideLength/2));
			float z = (float) ((random.NextDouble() * sideLength) - (sideLength/2));
			int waitTimeMilliseconds = 2000;
			IAction action = ActionBuilder.Start(pawnController, () => {})
										.Animation(AnimationName.Idle)
										.AnimationPlayLength(waitTimeMilliseconds)
										.Finish();
			Predicate<Vector3> predicate = FogController.GetFogController().IsInbounds;
			ITargeting targeting = new StaticPointTargeting(new Vector3(x,5,z), predicate);
			return new Task(targeting, action);
		}
	}
}
