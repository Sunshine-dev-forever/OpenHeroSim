using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Actions;
using System.Linq;
using Godot;

public class ActionController
{
	struct ActionStruct {
		public ActionStruct(IAction _action, DateTime _timeLastUsed) {
			action = _action;
			timeLastUsed = _timeLastUsed;
		}
		public IAction action;
		public DateTime timeLastUsed;
	}

	private MultiThreadUtil multiThreadUtil = new MultiThreadUtil();

	private Dictionary<string, ActionStruct> actionsDict = new Dictionary<string, ActionStruct>();

	public ActionController() {
		IAction waitAction = new WaitAction();
		actionsDict.Add(waitAction.Name, new ActionStruct(waitAction, DateTime.MinValue));
		IAction stabAction = new StabAction();
		actionsDict.Add(stabAction.Name, new ActionStruct(stabAction, DateTime.MinValue));
	}

	public void ExecuteActionFromTask(ITask task, VisualController visualController) {
		if(actionsDict.ContainsKey(task.actionName)) {
			ActionStruct actionStruct = actionsDict[task.actionName];
			IAction action = actionStruct.action;
			//TODO: This HAS to be refactored
			actionsDict[task.actionName] = new ActionStruct(action, DateTime.Now);
			actionStruct.timeLastUsed = DateTime.Now;
			multiThreadUtil.Run(() => {action.execute(task.actionArgs, visualController);});
		}
	}

	public List<IAction> GetAllActionsWithTags(List<ActionTags> actionTags, bool canBeOnCooldown) {
		//Convert to dictionary values to IEnumerable<ActionStruct>
		IEnumerable<ActionStruct> actionStructs = actionsDict.Values.AsEnumerable();
		//Get all actions with the specified tags
		actionStructs = actionStructs.Where( (actionStruct) => actionStruct.action.Tags.Intersect(actionTags).Count() == actionTags.Count);
		//filter out on CD stuff
		if(!canBeOnCooldown) {
			actionStructs = actionStructs.Where( (actionStruct) =>  
												(DateTime.Now - actionStruct.timeLastUsed).TotalMilliseconds 
												> actionStruct.action.CooldownMilliseconds);
		}

		return actionStructs.Select((actionStruct) => actionStruct.action).ToList();
	}

	public void HandleTask(ITask task , MovementController movementController, VisualController visualController) {
		if(!task.isValid) {
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
		if(movementController.HasFinishedMovement(task.targetDistance)) {
			movementController.Stop();
			//we could get rid of the starting action state and just start he action here
			ExecuteActionFromTask(task, visualController);
			task.TaskState = TaskState.USING_ACTION;
		} else {
			//we also need to change animations
			AnimationPlayer animationPlayer = visualController.GetAnimationPlayer();
			animationPlayer.GetAnimation("Walking").Loop = true;
			//TODO: perhaps I should not be playing the same animation over and over again... does not seem to be an issue right now
			animationPlayer.Play("Walking");
		}
	}

	public bool IsActionCompleted() {
		return multiThreadUtil.IsTaskCompleted();
	}
}
