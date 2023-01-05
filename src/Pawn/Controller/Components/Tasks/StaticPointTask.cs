using Godot;
using System.Collections.Generic;
using Pawn.Action;

namespace Pawn.Tasks {
	public class StaticPointTask : ITask {
		private Vector3 targetPoint;

		public StaticPointTask(IAction _action,
								Vector3 _targetPoint) 
		{
			Action = _action;
			targetPoint = _targetPoint;
			TaskState = TaskState.MOVING_TO;
		}
		public Vector3 GetTargetLocation(){
			return targetPoint;
		}
		public IAction Action{get;}
		//Represents whether the task is valid or not
		public bool IsValid {get {return true;}}
		public bool IsInterruptable {get;}
		public int Priority{get; set;}
		public TaskState TaskState {get; set;}
	}
}