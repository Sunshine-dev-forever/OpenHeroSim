using System.Collections.Generic;
using System.Threading;
using System;
using Serilog;
using Pawn;
namespace Pawn.Action.Ability {
	public interface IAbility : IAction {
		public IAbility Duplicate(PawnController ownerPawnController, PawnController otherPawnController);
		public AnimationName AnimationToPlay {get;}
		//in the future I am going to want abillities that can only be used against certtain factions or something
		//So I am passing otherPawnController here
		public Boolean CanBeUsed(PawnController ownerPawnController, PawnController otherPawnController);
	}
}