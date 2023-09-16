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

namespace Pawn.Goal
{
	public class DefendSelfGoal : IPawnGoal
	{
		//TODO: break this up into smaller functions
		public ITask GetTask(IPawnController ownerPawnController, SensesStruct sensesStruct)
		{
			Func<IPawnController, bool> pawnIsAliveAndValid = (pawnController) =>
			{
				return pawnController != null && pawnController.IsInstanceValid() && !pawnController.IsDying;
			};
			List<IPawnController> nearbyLivingPawns = sensesStruct.nearbyPawns.AsEnumerable().Where(pawnIsAliveAndValid).ToList();
			if (nearbyLivingPawns.Count == 0)
			{
				return new InvalidTask();
			}
			IPawnController? pawnToAttack = null;
			//need to get the nearest pawn on the right faction
			foreach (IPawnController pawn in nearbyLivingPawns)
			{
				string otherFaction = pawn.PawnInformation.Faction;
				string ownerFaction = ownerPawnController.PawnInformation.Faction;
				if (ownerFaction.Equals(IPawnInformation.NO_FACTION) || (!ownerFaction.Equals(otherFaction)))
				{
					pawnToAttack = pawn;
					break;
				}
			}
			if (pawnToAttack == null)
			{
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
				IAction waitAction = ActionBuilder.Start(ownerPawnController, () => { })
										.Animation(AnimationName.Idle)
										.AnimationPlayLength(waitTimeMilliseconds)
										.Finish();
				return new Task(targeting, waitAction, "waiting for any ability to become usable");
			}
			else
			{
				//We take first valid ability
				IAbility ability = validAbilities[0];
				ability.Setup(pawnToAttack);
				return new Task(targeting, ability, String.Format("Attacking the pawn {0} with the ability {1}", pawnToAttack.PawnInformation.Name, ability.Name));
			}
		}
	}
}
