using Godot;
using System.Collections.Generic;
using Pawn.Action;

namespace Pawn.Tasks {
	public class StaticPointTask : ITask {
		private Vector3 targetPoint;

		public StaticPointTask(IAction _action,
								float _targetDistance,
								Vector3 _targetPoint,
								bool _IsCombat = false) 
		{
			Action = _action;
			TargetDistance = _targetDistance;
			targetPoint = _targetPoint;
			TaskState = TaskState.MOVING_TO;
			IsCombat = _IsCombat;
		}
		public Vector3 GetTargetLocation(){
			return targetPoint;
		}
		public IAction Action{get;}
		//How close the pawn will attempt to get to the target before starting the action
		public float TargetDistance {get;}
		//Represents whether the task is valid or not
		public bool IsValid {get {return true;}}
		public bool IsCombat {get;}
		public TaskState TaskState {get; set;}
	}
}