
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Godot;
using Pawn;
using Pawn.Action.Ability;
using Item;

namespace Pawn.Action {
	public class ActionBuilder {
		private Action action;

		private ActionBuilder(PawnController ownerPawnController, System.Action executable) {
			action = new Action(ownerPawnController, executable);
		}
		public static ActionBuilder Start(PawnController ownerPawnController, System.Action executable) {
			return new ActionBuilder(ownerPawnController, executable);
		}

		//CRNT: this method will no longer be needed
		// public static ActionBuilder Start(IAbility ability, PawnController pawnController) {
		// 	ActionBuilder actionBuilder = new ActionBuilder(pawnController, ability.execute);
		// 	return actionBuilder.Animation(ability.AnimationToPlay)
		// 			.AddTags(ability.Tags)
		// 			.MaxRange(ability.MaxRange)
		// 			.Name(ability.Name)
		// 			.HeldItem(ability.HeldItem);
		// }

		public ActionBuilder MaxRange(float range) {
			action.MaxRange = range;
			return this;
		}

		public ActionBuilder Animation(AnimationName animationName) {
			action.AnimationToPlay = animationName;
			return this;
		}

		//Sets looping to be true
		public ActionBuilder AnimationPlayLength(int milliseconds) {
			action.SetAnimationPlayLength(milliseconds);
			return this;
		}

		public ActionBuilder HeldItem(IItem? item) {
			action.HeldItem = item;
			return this;
		}

		public IAction Finish() {
			return action;
		}

		public ActionBuilder Name(string name) {
			action.Name = name;
			return this;
		}
		
	}
}