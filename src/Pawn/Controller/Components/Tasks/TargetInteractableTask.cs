using Godot;
using System;
using Pawn.Controller;
using Pawn.Action;

namespace Pawn.Tasks {
	public class TargetInteractableTask : ITask {
		private IInteractable targetInteractable;

		public TargetInteractableTask(IAction _action,
								float _targetDistance,
								IInteractable _targetInteractable
								) 
		{
			Action = _action;
			TargetDistance = _targetDistance;
			targetInteractable = _targetInteractable;
			TaskState = TaskState.MOVING_TO;
		}
		public Vector3 GetTargetLocation(){
			if(!this.IsValid){
				throw new NullReferenceException("task was not valid");
			}
			return targetInteractable.GlobalTransform.origin;
		}
		public IAction Action {get;}
		//How close the pawn will attempt to get to the target before starting the action
		public float TargetDistance {get;}
		//Represents whether the task is valid or not
		//This should be smart enough to not go off on null pointer error
		public bool IsValid {get {return (targetInteractable == null || targetInteractable.IsInstanceValid());}}
		public TaskState TaskState {get; set;}
		public int Priority {get; set;}
	}
}