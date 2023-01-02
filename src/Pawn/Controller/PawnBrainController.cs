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
		private List<IPawnGoal> goals = new List<IPawnGoal>();
		private ActionController actionController;

		//TODO: implement a combat goal list (combat goals would be like heal, save ally, kill, etc)
		//private List<IPawnGoal> combatGoalList = new List<IPawnGoal>();

		public PawnBrainController(ActionController _actionController)
		{
			actionController = _actionController;
		}

		public void AddGoal(IPawnGoal goal) {
			goals.Add(goal);
		}
		
		//TODO: I would prefer to not take in pawnController here
		//But I need to because tasks need a refernce to the pawnController
		public ITask updateCurrentTask(ITask currentTask, SensesStruct sensesStruct, PawnController pawnController)
		{
			if (currentTask.TaskState == TaskState.COMPLETED || !currentTask.IsValid)
			{
				//if the current task is done or invalid then we get a new task no matter what
				return GetNextTask(pawnController, sensesStruct);
			}
			else
			{
				//otherwise we try to create a higher priority task
				return GetHigherPriorityTaskOrCurrentTask(currentTask, sensesStruct,pawnController);
			}
		}

		public ITask GetHigherPriorityTaskOrCurrentTask(ITask currentTask, SensesStruct sensesStruct, PawnController pawnController) {
			//Lower index means HigherPriority
			for (int i = 0; i < goals.Count; i++)
			{
				if(i >= currentTask.Priority) {
					//Only want tasks will a Priority that could be higher
					break;
				}
				IPawnGoal pawnGoal = goals[i];
				ITask nextTask = pawnGoal.GetTask(pawnController, sensesStruct);
				nextTask.Priority = i;
				if (nextTask.IsValid )
				{
					return nextTask;
				}
			}
			return currentTask;
		}

		public ITask GetNextTask(PawnController pawnController, SensesStruct sensesStruct)
		{
			for (int i = 0; i < goals.Count; i++)
			{
				IPawnGoal pawnGoal = goals[i];
				ITask nextTask = pawnGoal.GetTask(pawnController, sensesStruct);
				nextTask.Priority = i;
				if (nextTask.IsValid)
				{
					return nextTask;
				}
			}
			return new InvalidTask();
		}

		private bool isHostilePawnsInVision(SensesStruct sensesStruct)
		{
			//TODO: for right now, if any pawns are nearby they are hostile
			return sensesStruct.nearbyPawns.Count > 0;
		}
	}
}