using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn;
using Pawn.Action.Ability;
using System.Linq;
using Godot;

namespace Pawn {
	public class PawnTaskHandler
	{

		private IAction? actionInExecution;
		private PawnMovement movementController;
		private PawnVisuals visualController;
		private IPawnInformation pawnInformation;
		private IPawnInventory pawnInventory;

		public PawnTaskHandler(PawnMovement _movementController, PawnVisuals _visualController, IPawnInformation _pawnInformation,
					IPawnInventory _pawnInventory) {
			movementController = _movementController;
			visualController = _visualController;
			pawnInformation = _pawnInformation;
			pawnInventory = _pawnInventory;
		}

		public void HandleTask(ITask task, IPawnController ownerPawnController) {
			if(!task.IsValid) {
				//early exit on invalid task
				return;
			}
			switch (task.TaskState) {
				case TaskState.MOVING_TO: 
					MoveToTaskLocation(task);
				break;

				case TaskState.USING_ACTION: 
					if(IsActionCompleted()) {
						task.TaskState = TaskState.COMPLETED;
					}
				break;

				case TaskState.COMPLETED: 
				break;
			}
		}

		private void MoveToTaskLocation(ITask task) {
			//I have to call Process movement first so movement controller can update the final location
			//TODO: refactor so I dont call process movement first (movementController has to update the final location)
			movementController.ProcessMovement(task.GetTargetPosition(), pawnInformation.Speed);
			
			if(movementController.HasFinishedMovement(task.Action.MaxRange)) {
				//if movement is finished then we start the action
				movementController.Stop();
				
				actionInExecution = task.Action;
				actionInExecution.Execute();
				
				task.TaskState = TaskState.USING_ACTION;
				visualController.UpdateHeldItem(task.Action.HeldItem, pawnInventory);
			} else {
				//otherwise we are just walking
				visualController.SetAnimation(AnimationName.Walking, true);
			}
		}

		public bool IsActionCompleted() {
			//defaults to true if action is null
			if(actionInExecution == null) {
				return true;
			}
			return actionInExecution.IsFinished();
		}
	}
}
