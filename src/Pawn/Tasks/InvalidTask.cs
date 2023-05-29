using Godot;
using System.Collections.Generic;
using Pawn.Action;

namespace Pawn.Tasks {
	public class InvalidTask : ITask {
		private Vector3 targetPoint;

		public InvalidTask() 
		{
			IsValid = false;
			Action = null!;
			TaskState = TaskState.COMPLETED;
		}
		public Vector3 GetTargetPosition(){
			return targetPoint;
		}
		public IAction Action {get;}
		//How close the pawn will attempt to get to the target before starting the action
		public float TargetDistance {get;}
		//Represents whether the task is valid or not
		public bool IsValid {get;}
		public TaskState TaskState {get; set;}
		public int Priority {get; set;}
	}
}