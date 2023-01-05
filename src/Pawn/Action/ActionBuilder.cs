
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Godot;
using Pawn.Controller;
using Pawn.Action.Ability;

namespace Pawn.Action {
	public class ActionBuilder {
		private Action action;

		private ActionBuilder(PawnController pawnController, System.Action executable) {
			action = new Action(pawnController, executable);
		}
		public static ActionBuilder Start(PawnController pawnController, System.Action executable) {
			return new ActionBuilder(pawnController, executable);
		}

		public static ActionBuilder Start(IAbility ability, PawnController pawnController) {
			ActionBuilder actionBuilder = new ActionBuilder(pawnController, ability.execute);
			return actionBuilder.Animation(ability.AnimationToPlay)
					.AddTags(ability.Tags)
					.MaxRange(ability.MaxRange)
					.Name(ability.Name)
					.HeldItemMesh(ability.HeldItemMesh);
		}

		public ActionBuilder AddTag(ActionTags actionTag) {
			action.Tags.Add(actionTag);
			return this;
		}

		public ActionBuilder AddTags(IEnumerable<ActionTags> tags) {
			action.Tags.AddRange(tags);
			return this;
		}

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

		public ActionBuilder HeldItemMesh(Spatial mesh) {
			action.HeldItemMesh = mesh;
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