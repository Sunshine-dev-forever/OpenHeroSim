using System.Collections.Generic;
using Godot;
using GUI.DebugInspector.Display;
using Pawn.Action;
using Pawn.Targeting;

namespace Pawn.Tasks
{
	public class Task : ITask
	{
		//TODO: Name is not fully accurate,
		//examples of values Name will have are:
		//"Waiting in place for ever and ever (DebugGoal)"
		//"healing with consumable"
		//"wandering about"
		public string Description { get; set; } = "default task name";
		ITargeting targeting;
		public Task(ITargeting _targeting, IAction action)
		{
			targeting = _targeting;
			Action = action;
		}

		public Task(ITargeting _targeting, IAction action, string description) : this(_targeting, action)
		{
			Description = description;
			TaskState = TaskState.MOVING_TO;
		}
		public Vector3 GetTargetPosition()
		{
			return targeting.GetTargetPosition();
		}
		public IAction Action { get; }
		//Represents whether the task is valid or not
		public bool IsValid
		{
			get
			{
				return targeting.IsValid;
			}
		}
		//The state that the task is in
		public TaskState TaskState { get; set; }
		//Tasks with Priority -1 will never get interruptted
		//Priority is the game as the goal postion in the goal list. 
		//Goals earlier in the list will have a lower priority
		public int Priority { get; set; }

		public IDisplay Display => ConstructDisplay();

		private IDisplay ConstructDisplay()
		{
			Display display = new Display("Task");
			display.AddDetail("Task: " + Description);
			display.AddDetail("task state: " + TaskState);
			display.AddDetail("Moving to Position: " + targeting.GetTargetPosition());
			return display;
		}
	}
}