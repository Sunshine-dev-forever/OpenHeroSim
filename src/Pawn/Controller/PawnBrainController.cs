using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Goal;

namespace Pawn.Controller
{
	public class PawnBrainController
	{
		public bool noCombat = false;
		private List<IPawnGoal> adventureGoalList = new List<IPawnGoal>();
		private ActionController actionController;

		//TODO: implement a combat goal list (combat goals would be like heal, save ally, kill, etc)
		//private List<IPawnGoal> combatGoalList = new List<IPawnGoal>();

		public PawnBrainController(ActionController _actionController)
		{
			actionController = _actionController;
		}

		public void AddGoal(IPawnGoal goal) {
			adventureGoalList.Add(goal);
		}
		
		//TODO: I would prefer to not take in pawnController here
		//But I need to because tasks need a refernce to the pawnController
		public ITask updateCurrentTask(ITask currentTask, SensesStruct sensesStruct, PawnController pawnController)
		{

			if (isHostilePawnsInVision(sensesStruct) && !noCombat)
			{
				//we are in combat
				if (!currentTask.IsCombat || currentTask.TaskState == TaskState.COMPLETED || !currentTask.IsValid)
				{
					//task is non-combat or taskState is completed or task is not valid
					//then we neeed a new task
					return GetNextCombatTask(pawnController, sensesStruct.nearbyPawns[0]);
				}
				else
				{
					return currentTask;
				}
			}
			else
			{
				//we are not in comabt
				if (currentTask.TaskState == TaskState.COMPLETED || !currentTask.IsValid)
				{
					Log.Information(pawnController.pawnName + ": Created new goal");
					return GetNextTask(pawnController);
				}
				else
				{
					return currentTask;
				}
			}
		}

		public ITask GetNextTask(PawnController pawnController)
		{
			foreach (IPawnGoal pawnGoal in adventureGoalList)
			{
				ITask nextTask = pawnGoal.GetTask(pawnController);
				if (nextTask.IsValid)
				{
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
			if (validActions.Count < 1)
			{
				//if not actions are vaild, then we have to wait
				int waitTimeMilliseconds = 100;
				IAction waitAction = new WaitAction(pawnController, waitTimeMilliseconds);
				int FOLLOW_DISTNACE = 2;
				Log.Information(pawnController.pawnName + ":Created new wait combat goal");
				return new TargetPawnTask(waitAction, FOLLOW_DISTNACE, otherPawnController, true);
			}
			//This action has to be a stab action for now
			IAction action = validActions[0].Duplicate(pawnController, otherPawnController);
			Log.Information(pawnController.pawnName + ":Created new stab combat goal");
			ITask task = new TargetPawnTask(action, action.MaxRange, otherPawnController, true);
			return task;
		}

		private bool isHostilePawnsInVision(SensesStruct sensesStruct)
		{
			//TODO: for right now, if any pawns are nearby they are hostile
			return sensesStruct.nearbyPawns.Count > 0;
		}
	}
}