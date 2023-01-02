using System.Collections.Generic;
using System.Threading;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn.Action {
	public class StabAction : IAction {
		
		private PawnController? ownerPawnController;
		private PawnController? otherPawnController;

		public Godot.Spatial? HeldItemMesh {get; set;}
		public StabAction() {}
		public StabAction( PawnController _ownerPawnController, PawnController _otherPawnController){
			ownerPawnController = _ownerPawnController;
			otherPawnController = _otherPawnController;
			HeldItemMesh = ownerPawnController.Weapon.Mesh;
		}

		public IAction Duplicate(PawnController _ownerPawnController, PawnController _otherPawnController) {
			return new StabAction(_ownerPawnController, _otherPawnController);
		}

		public int CooldownMilliseconds {get {return 2000;} }
		//TODO implement tags for actions
		//object Tags {get;}
		public string Name {get {return "StabAction";}}
		public float MaxRange {get {return 2;}}

		private ActionTags[] tags = {ActionTags.COMBAT};
		public List<ActionTags> Tags {get {return new List<ActionTags>(tags);}}

		//@param waitTimeMilliseconds - amount of time to wait
		public void execute() {
			//TODO: Stab should provide its own multipliers
			double damage = ownerPawnController.GetDamage();
			otherPawnController.TakeDamage(damage);

			ownerPawnController.VisualController.SetAnimation(AnimationName.Stab);
			Thread.Sleep( (int) (ownerPawnController.VisualController.getAnimationLengthMilliseconds(AnimationName.Stab)) );
		}

	}
}