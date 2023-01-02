using Godot;
using System;
using Pawn.Controller;
using Pawn.Action;

namespace Pawn.Tasks {
	public class TargetPawnTask : ITask {
		private PawnController targetPawn;

		public TargetPawnTask(IAction _action,
								float _targetDistance,
								PawnController _targetPawn,
								bool _IsCombat = false) 
		{
			Action = _action;
			TargetDistance = _targetDistance;
			targetPawn = _targetPawn;
			TaskState = TaskState.MOVING_TO;
			IsCombat = _IsCombat;
		}
		public Vector3 GetTargetLocation(){
			if(!this.IsValid){
				throw new NullReferenceException("task was not valid");
			}
			return targetPawn.GlobalTransform.origin;
		}
		public IAction Action {get;}
		//How close the pawn will attempt to get to the target before starting the action
		public float TargetDistance {get;}
		//Represents whether the task is valid or not
		public bool IsValid {get {return Godot.Object.IsInstanceValid(targetPawn);}}
		public bool IsCombat {get;}
		public TaskState TaskState {get; set;}

	}
}