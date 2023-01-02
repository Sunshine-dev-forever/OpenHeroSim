using System;
using Godot;
using System.Collections.Generic;
using Pawn.Controller;

namespace Pawn.Action {
	public interface IAction {

		int CooldownMilliseconds {get; }
		//TODO implement tags for actions
		List<ActionTags> Tags {get;}
		string Name {get;}
		float MaxRange {get;}
		public IAction Duplicate(PawnController ownerPawnController, PawnController otherPawnController);
		void execute();
		public Spatial? HeldItemMesh {get; set;}
	}

	public enum ActionTags {COMBAT}
}