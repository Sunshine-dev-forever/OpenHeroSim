using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Action.Ability;
using Pawn;
using Item;
using Pawn.Targeting;
using System.Linq;
using Pawn.Components;

namespace Pawn.Goal {
	public class DefendSelfGoal : IPawnGoal
	{
		//TODO: break this up into smaller functions
		public ITask GetTask(PawnController ownerPawnController, SensesStruct sensesStruct) {
			Func<PawnController, bool> pawnIsAliveAndValid = (pawnController) => { 
				return pawnController != null && pawnController.IsInstanceValid() && !pawnController.IsDying;
			};
			List<PawnController> nearbyLivingPawns = sensesStruct.nearbyPawns.AsEnumerable().Where(pawnIsAliveAndValid).ToList();
			if(nearbyLivingPawns.Count == 0) {
				return new InvalidTask();
			}
			PawnController? pawnToAttack = null;
			//need to get the nearest pawn on the right faction
			foreach (PawnController pawn in nearbyLivingPawns) {
				string otherFaction = pawn.PawnInformation.Faction;
				string ownerFaction = ownerPawnController.PawnInformation.Faction;
				if(ownerFaction.Equals(PawnInformation.NO_FACTION) || (!ownerFaction.Equals(otherFaction)) ){
					pawnToAttack = pawn;
					break;
				}
			}
			if(pawnToAttack == null) {
				return new InvalidTask();
			}
			List<IAbility> validAbilities = ownerPawnController.PawnInformation.GetAllUsableAbilities(ownerPawnController, pawnToAttack);
			//no matter what we are targeting the other pawn
			ITargeting targeting = new InteractableTargeting(pawnToAttack);
			if (validAbilities.Count == 0)
			{
				//returning an invalid action here could cause the brain to move on to the next goal
				//Which is not what we want
				//Therefore the pawn will wait until an ability is usable
				//if not actions are vaild, then we have to wait
				int waitTimeMilliseconds = 100;
				IAction waitAction = ActionBuilder.Start(ownerPawnController, () => {})
										.Animation(AnimationName.Idle)
										.AnimationPlayLength(waitTimeMilliseconds)
										.Finish();
				//TODO: pawnController.Weapon.Mesh should default to a spatial node. even if Weapon is null
				waitAction.HeldItem = ownerPawnController.PawnInventory.GetWornEquipment(EquipmentType.HELD);
				return new Task(targeting, waitAction);
			} else {	
				//We take first valid ability
				IAbility ability = validAbilities[0];
				IItem? heldItem = ownerPawnController.PawnInventory.GetWornEquipment(EquipmentType.HELD);
				ability.Setup(pawnToAttack, heldItem);
				return new Task(targeting, ability);
			}
		}
	}
}
