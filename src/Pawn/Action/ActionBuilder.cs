
using System.Collections.Generic;
using Pawn.Controller;

namespace Pawn.Action {
	public class ActionBuilder {
		private Action action;

		private ActionBuilder(PawnController pawnController, System.Action executable) {
			action = new Action(new List<ActionTags>(), pawnController, executable);
		}
		public static ActionBuilder Start(PawnController pawnController, System.Action executable) {
			return new ActionBuilder(pawnController, executable);
		}

		public ActionBuilder AddTag(ActionTags actionTag) {
			action.Tags.Add(actionTag);
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

		public IAction Finish() {
			return action;
		}
		
	}
}