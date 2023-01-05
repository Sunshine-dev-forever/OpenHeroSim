using System;
using Godot;
using System.Collections.Generic;
using Pawn.Controller;

namespace Pawn.Action {
	public interface IAction {
		int CooldownMilliseconds {get; }
		List<ActionTags> Tags {get;}
		string Name {get;}
		float MaxRange {get;}
		void execute();
		public Spatial? HeldItemMesh {get; set;}
		public bool IsFinished();
	}

	public enum ActionTags {COMBAT}
}