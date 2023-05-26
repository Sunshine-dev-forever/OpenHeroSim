using System;
using Godot;
using System.Collections.Generic;
using Pawn;
using Item;
using Interactable;

namespace Pawn.Action {
	public interface IAction {
		string Name {get;}
		float MaxRange {get;}
		void Execute();
		public IItem? HeldItem {get; set;}
		public bool IsFinished();
		public AnimationName AnimationToPlay {get;}
	}
}