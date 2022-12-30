using Godot;
using System.Collections.Generic;

namespace Pawn.Tasks {
	public class InvalidTask : ITask {
		private Vector3 targetPoint;

		public InvalidTask() 
		{
			isValid = false;
			isCombat = false;
			actionName = "";
			actionArgs = "";
			TaskType = TaskType.REST;
			TaskState = TaskState.COMPLETED;
		}
		public Vector3 GetTargetLocation(){
			return targetPoint;
		}
		public string actionName {get;}
		//Dictionary for the action ;arguments
		public object actionArgs {get;}
		//How close the pawn will attempt to get to the target before starting the action
		public float targetDistance {get;}
		//Represents whether the task is valid or not
		public bool isValid {get;}
		public bool isCombat {get;}
		public TaskType TaskType {get;}
		public TaskState TaskState {get; set;}
	}
}