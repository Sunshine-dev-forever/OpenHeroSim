using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Actions;

public class PawnCombatBrain
{

	public ITask GetNextTask(PawnController pawnController, ActionController actionController, PawnController otherPawnController)
	{
		List<ActionTags> requestedTags = new List<ActionTags>();
		requestedTags.Add(ActionTags.COMBAT);
		List<IAction> validActions = actionController.GetAllActionsWithTags(requestedTags, false);
		//The only valid action here is stab action
		if(validActions.Count != 1) {
			WaitAction.WaitActionArgs waitArgs = new WaitAction.WaitActionArgs(500);
			return new TargetPawnTask("WaitAction", waitArgs, 2, otherPawnController);
		}
		//I am assuming this is a stab action
		IAction stabAction = validActions[0];
		StabAction.StabActionArgs actionArgs = new StabAction.StabActionArgs(otherPawnController, 10);
		ITask task = new TargetPawnTask("StabAction", actionArgs, 2, otherPawnController);
		return task;
	}
}
