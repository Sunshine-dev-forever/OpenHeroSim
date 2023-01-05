using System.Collections.Generic;
using System.Threading;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn.Action.Ability {
	public class StabAbility : IAction, IAbility {
		
		private PawnController? ownerPawnController;
		private PawnController? otherPawnController;

		public Godot.Spatial? HeldItemMesh {get; set;}
		public StabAbility() {}
		public StabAbility( PawnController _ownerPawnController, PawnController _otherPawnController){
			ownerPawnController = _ownerPawnController;
			otherPawnController = _otherPawnController;
			if(ownerPawnController.Weapon != null){
				HeldItemMesh = ownerPawnController.Weapon.Mesh;
			}
		}

		public IAbility Duplicate(PawnController _ownerPawnController, PawnController _otherPawnController) {
			return new StabAbility(_ownerPawnController, _otherPawnController);
		}

		public int CooldownMilliseconds {get {return 2000;} }
		public string Name {get {return "StabAction";}}
		public float MaxRange {get {return 2;}}
		private ActionTags[] tags = {ActionTags.COMBAT};
		public List<ActionTags> Tags {get {return new List<ActionTags>(tags);}}

		public void execute() {
			//TODO: Stab should provide its own multipliers
			double damage = ownerPawnController.GetDamage();
			otherPawnController.TakeDamage(damage);

			ownerPawnController.VisualController.SetAnimation(AnimationName.Stab);
			Thread.Sleep( (int) (ownerPawnController.VisualController.getAnimationLengthMilliseconds(AnimationName.Stab)) );
		}

	}
}