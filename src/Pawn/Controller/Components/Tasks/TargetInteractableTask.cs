using Godot;
using System;
using Pawn.Controller;
using Pawn.Action;

namespace Pawn.Tasks {
	public class TargetInteractableTask : ITask {
		private IInteractable targetInteractable;

		public TargetInteractableTask(IAction _action,
								IInteractable _targetInteractable) 
		{
			Action = _action;
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
		//Represents whether the task is valid or not
		//This should be smart enough to not go off on null pointer error
		public bool IsValid {get {return (targetInteractable == null || targetInteractable.IsInstanceValid());}}
		public TaskState TaskState {get; set;}
		public int Priority {get; set;}
	}
}