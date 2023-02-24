using System.Collections.Generic;
using Godot;
using Pawn.Action;

namespace Pawn.Targeting {
	public interface ITargeting {
		public Vector3 GetTargetLocation();
		public bool IsValid {get;}
	}
}