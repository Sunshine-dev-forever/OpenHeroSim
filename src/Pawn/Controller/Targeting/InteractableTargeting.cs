using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using Godot;
using Pawn.Action;

namespace Pawn.Targeting {
	public partial class InteractableTargeting : ITargeting {
		private Vector3 offset;
		private IInteractable Interactable {get;}
		public InteractableTargeting(IInteractable interactable) {
			Interactable = interactable;
		}
		public InteractableTargeting(IInteractable interactable, Vector3 _offset) {
			offset = _offset;
			Interactable = interactable;
		}
		public Vector3 GetTargetPosition() {
			return Interactable.GlobalTransform.Origin + offset;
		}
		public bool IsValid { get {
			return Interactable.IsInstanceValid();
		} }
	}
}