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
		//is this task a combat task (will this task be interuppted by combat?)
		bool isCombat {get;}
		//Unused for now, I should probablly delete TaskType
		TaskType TaskType {get;}
		//The state that the task is in
		TaskState TaskState {get; set;}
		
	}
	//Might not want to care about task type, only care if combat/non-combat for now
	public enum TaskType { COMBAT, REST, ADVENTURE};
	//The status that the task is in
	public enum TaskState { MOVING_TO, USING_ACTION, COMPLETED}
}