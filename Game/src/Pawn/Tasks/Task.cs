using System.Collections.Generic;
using Godot;
using Pawn.Action;
using Pawn.Targeting;

namespace Pawn.Tasks {
	public class Task : ITask {
		ITargeting targeting;
		public Task(ITargeting _targeting, IAction action) {
			targeting = _targeting;
			Action = action;
			TaskState = TaskState.MOVING_TO;
		}
		public Vector3 GetTargetPosition() {
			return targeting.GetTargetPosition();
		}
		public IAction Action {get;}
		//Represents whether the task is valid or not
		public bool IsValid {get {
			return targeting.IsValid;
		}}
		//The state that the task is in
		public TaskState TaskState {get; set;}
		//Tasks with Priority -1 will never get interruptted
		//Priority is the game as the goal postion in the goal list. 
		//Goals earlier in the list will have a lower priority
		public int Priority {get; set;}
		
	}
}