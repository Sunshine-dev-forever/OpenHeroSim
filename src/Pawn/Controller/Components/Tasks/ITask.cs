using System.Collections.Generic;
using Godot;
using Pawn.Action;

namespace Pawn.Tasks {
	public interface ITask {
		Vector3 GetTargetLocation();
		//The name of the action to be used
		IAction Action {get;}
		//Dictionary for the action ;arguments
		float TargetDistance {get;}
		//Represents whether the task is valid or not
		bool IsValid {get;}
		//is this task a combat task (will this task be interuppted by combat?)
		bool IsCombat {get;}
		//The state that the task is in
		TaskState TaskState {get; set;}
		
	}
	//The status that the task is in
	public enum TaskState { MOVING_TO, USING_ACTION, COMPLETED}
}