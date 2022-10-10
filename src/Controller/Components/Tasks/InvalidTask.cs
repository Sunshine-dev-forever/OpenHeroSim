using Godot;
using System.Collections.Generic;

namespace Pawn.Tasks {
	public class InvalidTask : ITask {
		private Vector3 targetPoint;

		public InvalidTask() 
		{
			isValid = false;
			actionName = "";
			actionArgs = "";
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
	}
}