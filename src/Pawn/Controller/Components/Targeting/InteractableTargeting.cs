using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using Godot;
using Pawn.Action;

namespace Pawn.Targeting {
	public class InteractableTargeting : ITargeting {
		private IInteractable Interactable {get;}
		public InteractableTargeting(IInteractable interactable) {
			Interactable = interactable;
		}
		public Vector3 GetTargetLocation() {
			return Interactable.GlobalTransform.origin;
		}
		public bool IsValid { get {
			return Interactable.IsInstanceValid();
		} }
	}
}