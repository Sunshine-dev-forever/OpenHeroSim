using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
namespace Pawn.Goal {
	public class DefendSelfGoal : IPawnGoal
	{
		public ITask GetTask(PawnController pawnController, SensesStruct sensesStruct) {

			if(sensesStruct.nearbyPawns.Count == 0) {
				return new InvalidTask();
			}
			PawnController otherPawnController = sensesStruct.nearbyPawns[0];
			List<ActionTags> requestedTags = new List<ActionTags>();
			requestedTags.Add(ActionTags.COMBAT);
			List<IAction> validActions = pawnController.actionController.GetAllActionsWithTags(requestedTags, false);
			
			//The only valid action in combat is stabbing
			if (validActions.Count < 1)
			{
				//if not actions are vaild, then we have to wait
				int waitTimeMilliseconds = 100;
				IAction waitAction = new WaitAction(pawnController, waitTimeMilliseconds);
				//TODO: pawnController.Weapon.Mesh should default to a spatial node. even if Weapon is null
				waitAction.HeldItemMesh = pawnController.Weapon.Mesh;
				int FOLLOW_DISTNACE = 2;
				return new TargetPawnTask(waitAction, FOLLOW_DISTNACE, otherPawnController);
			}
			//This action has to be a stab action for now
			IAction action = validActions[0].Duplicate(pawnController, otherPawnController);
			ITask task = new TargetPawnTask(action, action.MaxRange, otherPawnController);
			return task;
		}
	}
}
