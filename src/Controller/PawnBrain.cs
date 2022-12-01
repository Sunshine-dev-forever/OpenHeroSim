using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Actions;

public class PawnBrain
{
	private List<IPawnGoal> adventureGoalList = new List<IPawnGoal>();
	private ActionController actionController;
	private LoggerUtil logger = new LoggerUtil();
	
	//TODO: implement a combat goal list (combat goals would be like heal, save ally, kill, etc)
	//private List<IPawnGoal> combatGoalList = new List<IPawnGoal>();

	//TODO: properly create goals
	public PawnBrain(ActionController _actionController) {
		actionController = _actionController;
		adventureGoalList.Add(new WanderGoal());
	}
	//TODO: I would prefer to not take in pawnController here
	public ITask updateCurrentTask(ITask currentTask, SensesStruct sensesStruct, PawnController pawnController) {
		
		if(isHostilePawnsInVision(sensesStruct)) {
			//we are in combat
			if(!currentTask.isCombat || currentTask.TaskState == TaskState.COMPLETED || !currentTask.isValid) {
				//task is non-combat or taskState is completed or task is not valid
				//then we neeed a new task
				return GetNextCombatTask(pawnController, sensesStruct.nearbyPawns[0]);
			} else {
				return currentTask;
			}
		} else {
			//we are not in comabt
			if(currentTask.TaskState == TaskState.COMPLETED || !currentTask.isValid) {
				return GetNextTask(pawnController);
			} else {
				return currentTask;
			}
		}
	}

	public ITask GetNextTask(PawnController pawnController) {
		foreach (IPawnGoal pawnGoal in adventureGoalList) {
			ITask nextTask = pawnGoal.GetTask(pawnController);
			if(nextTask.isValid) {
				return nextTask;
			}
		}
		return new InvalidTask();
	}

	public ITask GetNextCombatTask(PawnController pawnController, PawnController otherPawnController)
	{
		List<ActionTags> requestedTags = new List<ActionTags>();
		requestedTags.Add(ActionTags.COMBAT);
		List<IAction> validActions = actionController.GetAllActionsWithTags(requestedTags, false);
		//The only valid action in combat is stabbing
		if(validActions.Count < 1) {
			//if not actions are vaild, then we have to wait
			WaitAction.WaitActionArgs waitArgs = new WaitAction.WaitActionArgs(500);
			return new TargetPawnTask("WaitAction", waitArgs, 2, otherPawnController, TaskType.COMBAT);
		}
		//I am assuming this is a stab action
		//TODO should properly check to see which kind of action this is
		IAction stabAction = validActions[0];
		StabAction.StabActionArgs actionArgs = new StabAction.StabActionArgs(otherPawnController, 10, pawnController);
		ITask task = new TargetPawnTask("StabAction", actionArgs, 2, otherPawnController, TaskType.COMBAT);
		return task;
	}

	private bool isHostilePawnsInVision(SensesStruct sensesStruct) {
		//TODO: for right now, if any pawns are nearby they are hostile
		return sensesStruct.nearbyPawns.Count > 0;
	}
}
