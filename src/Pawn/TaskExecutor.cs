using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
using Pawn.Action.Ability;
using System.Linq;
using Godot;

namespace Pawn {
	public class TaskExecutor
	{

		private IAction? actionInExecution;
		private MovementController movementController;
		private VisualController visualController;
		private PawnInformation pawnInformation;
		private PawnInventory pawnInventory;

		public TaskExecutor(MovementController _movementController, VisualController _visualController, PawnInformation _pawnInformation,
					PawnInventory _pawnInventory) {
			movementController = _movementController;
			visualController = _visualController;
			pawnInformation = _pawnInformation;
			pawnInventory = _pawnInventory;
		}

		public void ExecuteActionFromTask(ITask task) {
			//TODO: what if I create an action that happens to have an identical name as an ability?
			if(pawnInformation.HasAbility(task.Action.Name)) {
				//TODO: I cannot convert the action back into an ability so I must reference the old ability
				pawnInformation.UpdateAbilityLastUsed(task.Action.Name, DateTime.Now);
			}
			actionInExecution = task.Action;
			actionInExecution.execute();
		}

		public void HandleTask(ITask task, PawnInventory pawnInventory) {
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
			//I have to call Process movement first
			//TODO: refactor so I dont call process movement first (movementController has to update the final location)
			movementController.ProcessMovement(task.GetTargetLocation(), pawnInformation.Speed);
			if(movementController.HasFinishedMovement(task.Action.MaxRange)) {
				movementController.Stop();
				
				ExecuteActionFromTask(task);
				task.TaskState = TaskState.USING_ACTION;
				visualController.UpdateHeldItem(task.Action.HeldItem, pawnInventory);
			} else {
				//we also need to change animations
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
