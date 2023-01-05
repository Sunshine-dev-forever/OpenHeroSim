using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Action.Ability;
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
			List<IAbility> validAbilities = pawnController.ActionController.GetAllActionsWithTags(requestedTags, false);
			
			//The only valid action in combat is stabbing
			if (validAbilities.Count == 0)
			{
				//if not actions are vaild, then we have to wait
				int waitTimeMilliseconds = 100;
				IAction waitAction = ActionBuilder.Start(pawnController, () => {})
										.Animation(AnimationName.Idle)
										.AnimationPlayLength(waitTimeMilliseconds)
										.Finish();
				//TODO: pawnController.Weapon.Mesh should default to a spatial node. even if Weapon is null
				if(pawnController.Weapon != null) {
					waitAction.HeldItemMesh = pawnController.Weapon.Mesh;
				}
				return new TargetInteractableTask(waitAction, otherPawnController);
			}
			//This action has to be a stab action for now
			IAction action = validAbilities[0].Duplicate(pawnController, otherPawnController);
			ITask task = new TargetInteractableTask(action, otherPawnController);
			return task;
		}
	}
}
