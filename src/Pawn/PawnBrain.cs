using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Goal;
using Pawn.Components;

namespace Pawn
{
	//the pawn brain creates tasks for the rest of the pawn to execute.
	//The pawn brain has a list of goals which it iterates through to generate a task
	//goals earlier in the goal list have a higher priority and take precedent over the later goals
	public class PawnBrain
	{
		private List<IPawnGoal> goals = new List<IPawnGoal>();

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

		private ITask GetHigherPriorityTaskOrCurrentTask(ITask currentTask, SensesStruct sensesStruct, PawnController pawnController) {
			//Lower index means HigherPriority
			for (int i = 0; i < goals.Count; i++)
			{
				if(i >= currentTask.Priority) {
					//Only want tasks with a Priority that could be higher
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

		private ITask GetNextTask(PawnController pawnController, SensesStruct sensesStruct)
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
			//TODO: should return a "wait for 100 milliseconds task" so that hte pawncontroller does not constantly 
			//ask for a new task
			return new InvalidTask();
		}
	}
}