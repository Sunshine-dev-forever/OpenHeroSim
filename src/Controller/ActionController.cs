using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Actions;
using System.Linq;
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
	public void executeActionFromTask(ITask task) {
		if(actionsDict.ContainsKey(task.actionName)) {
			ActionStruct actionStruct = actionsDict[task.actionName];
			IAction action = actionStruct.action;
			//TODO: This HAS to be refactored
			actionsDict[task.actionName] = new ActionStruct(action, DateTime.Now);
			actionStruct.timeLastUsed = DateTime.Now;
			multiThreadUtil.Run(() => {action.execute(task.actionArgs);});
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

	public bool isActionCompleted() {
		return multiThreadUtil.IsTaskCompleted();
	}
}
