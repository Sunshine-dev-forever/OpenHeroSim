using Godot;
using System;

namespace Pawn.Tasks {
	public class TargetPawnTask : ITask {
		private PawnController targetPawn;

		public TargetPawnTask(string _actionName,
								object _actionArgs,
								float _targetDistance,
								PawnController _targetPawn,
								TaskType _TaskType) 
		{
			actionName = _actionName;
			actionArgs = _actionArgs;
			targetDistance = _targetDistance;
			targetPawn = _targetPawn;
			TaskType = _TaskType;
			TaskState = TaskState.MOVING_TO;
		}
		public Vector3 GetTargetLocation(){
			if(!this.isValid){
				throw new NullReferenceException("task was not valid");
			}
			return targetPawn.GlobalTransform.origin;
		}
		public string actionName {get;}
		//Dictionary for the action ;arguments
		public object actionArgs {get;}
		//How close the pawn will attempt to get to the target before starting the action
		public float targetDistance {get;}
		//Represents whether the task is valid or not
		public bool isValid {get {return Godot.Object.IsInstanceValid(targetPawn);}}
		public bool isCombat {get { return TaskType == TaskType.COMBAT;}}
		public TaskType TaskType {get;}
		public TaskState TaskState {get; set;}

	}
}