using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;

public class PawnBrain
{
	private List<IPawnGoal> goalList = new List<IPawnGoal>();

	//TODO: properly create goals
	public PawnBrain() {
		goalList.Add(new WanderGoal());
	}

	public ITask GetNextTask(PawnController pawnController) {
		foreach (IPawnGoal pawnGoal in goalList) {
			ITask nextTask = pawnGoal.GetTask(pawnController);
			if(nextTask.isValid) {
				return nextTask;
			}
		}
		return new InvalidTask();
	}
}
