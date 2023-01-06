using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Action.Ability;
using Pawn.Controller;
using Pawn.Item;

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
				
				waitAction.HeldItem = pawnController.PawnInventory.GetWornEquipment(EquipmentType.HELD);
				return new TargetInteractableTask(waitAction, otherPawnController);
			}
			//This action has to be a stab action for now
			IAbility ability = validAbilities[0].Duplicate(pawnController, otherPawnController);
			IAction action = ActionBuilder.Start(ability, pawnController).Finish();
			ITask task = new TargetInteractableTask(action, otherPawnController);
			return task;
		}
	}
}
