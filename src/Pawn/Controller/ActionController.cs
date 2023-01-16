using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Action.Ability;
using System.Linq;
using Godot;

namespace Pawn.Controller{
	public class ActionController
	{
		struct AbilityStruct {
			public AbilityStruct(IAbility _ability, DateTime _timeLastUsed) {
				ability = _ability;
				timeLastUsed = _timeLastUsed;
			}
			public IAbility ability;
			public DateTime timeLastUsed;
		}

		private IAction? actionInExecution;

		//Keeps track of cooldowns
		//TODO: this should be handled in the pawn information class
		private Dictionary<string, AbilityStruct> abilitiesDict = new Dictionary<string, AbilityStruct>();

		public ActionController() {
			//none of this should be needed!
			IAbility stabAction = new StabAbility();
			abilitiesDict.Add(stabAction.Name, new AbilityStruct(stabAction, DateTime.MinValue));
		}

		public void AddAbility(IAbility ability) {
			if(!abilitiesDict.ContainsKey(ability.Name)) {
				abilitiesDict.Add(ability.Name, new AbilityStruct(ability, DateTime.MinValue));
			}
		}

		public void ExecuteActionFromTask(ITask task, VisualController visualController) {
			if(abilitiesDict.ContainsKey(task.Action.Name)) {
				//TODO: I cannot convert the action back into an ability so I must reference the old ability
				abilitiesDict[task.Action.Name] = new AbilityStruct(abilitiesDict[task.Action.Name].ability, DateTime.Now);
			}
			actionInExecution = task.Action;
			actionInExecution.execute();
		}

		public List<IAbility> GetAllActionsWithTags(List<ActionTags> actionTags, bool canBeOnCooldown) {
			//Convert to dictionary values to IEnumerable<ActionStruct>
			IEnumerable<AbilityStruct> actionStructs = abilitiesDict.Values.AsEnumerable();
			//Get all actions with the specified tags
			actionStructs = actionStructs.Where( (actionStruct) => actionStruct.ability.Tags.Intersect(actionTags).Count() == actionTags.Count);
			//filter out on CD stuff
			if(!canBeOnCooldown) {
				actionStructs = actionStructs.Where( (actionStruct) =>  
													(DateTime.Now - actionStruct.timeLastUsed).TotalMilliseconds 
													> actionStruct.ability.CooldownMilliseconds);
			}

			return actionStructs.Select((actionStruct) => actionStruct.ability).ToList();
		}

		public void HandleTask(ITask task , MovementController movementController, VisualController visualController) {
			if(!task.IsValid) {
				//early exit on invalid task
				return;
			}
			switch (task.TaskState) {
				case TaskState.MOVING_TO: 
					MoveToTaskLocation(task, movementController, visualController);
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

		private void MoveToTaskLocation(ITask task , MovementController movementController, VisualController visualController) {
			//I have to call Process movement first
			//TODO: refactor so I dont call process movement first (movementController has to update the final location)
			int speed = 10;
			movementController.ProcessMovement(task.GetTargetLocation(), speed);
			if(movementController.HasFinishedMovement(task.Action.MaxRange)) {
				movementController.Stop();
				//we could get rid of the starting action state and just start he action here
				ExecuteActionFromTask(task, visualController);
				task.TaskState = TaskState.USING_ACTION;
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
