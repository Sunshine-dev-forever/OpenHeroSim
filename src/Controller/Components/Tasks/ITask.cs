using System.Collections.Generic;
using Godot;
namespace Pawn.Tasks {
	public interface ITask {
		Vector3 GetTargetLocation();
		//The name of the action to be used
		string actionName {get;}
		//Dictionary for the action ;arguments
		object actionArgs {get;}
		//How close the pawn will attempt to get to the target before starting the action
		float targetDistance {get;}
		//Represents whether the task is valid or not
		bool isValid {get;}
	}
}