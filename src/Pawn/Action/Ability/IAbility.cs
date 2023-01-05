using System.Collections.Generic;
using System.Threading;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn.Action.Ability {
	public interface IAbility : IAction {
		public IAbility Duplicate(PawnController ownerPawnController, PawnController otherPawnController);
		public AnimationName AnimationToPlay {get;}
	}
}