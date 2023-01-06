using System;
using Godot;
using System.Collections.Generic;
using Pawn.Controller;
using Pawn.Item;

namespace Pawn.Action {
	public interface IAction {
		int CooldownMilliseconds {get; }
		List<ActionTags> Tags {get;}
		string Name {get;}
		float MaxRange {get;}
		void execute();
		public IItem? HeldItem {get; set;}
		public bool IsFinished();
	}

	public enum ActionTags {COMBAT}
}