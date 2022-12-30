using System.Collections.Generic;
using System.Threading;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn.Action {
	public class StabAction : IAction {

		public int CooldownMilliseconds {get {return 2000;} }
		//TODO implement tags for actions
		//object Tags {get;}
		public string Name {get {return "StabAction";}}
		public float MaxRange {get {return 2;}}

		private ActionTags[] tags = {ActionTags.COMBAT};
		public List<ActionTags> Tags {get {return new List<ActionTags>(tags);}}

		//@param waitTimeMilliseconds - amount of time to wait
		public void execute(object argsStruct, VisualController visualController) {
			StabActionArgs args = (StabActionArgs) argsStruct;
			PawnController otherPawnController = args.otherPawnController;
			float damage = args.damage;
			otherPawnController.TakeDamage(damage);

			Godot.AnimationPlayer animationPlayer = visualController.GetAnimationPlayer();
			animationPlayer.GetAnimation("Stab").Loop = true;
			animationPlayer.Play("Stab");
			//TODO: find some constants library that defines this
			int MILLISECONDS_IN_SECOND = 1000;
			Thread.Sleep( (int) (animationPlayer.GetAnimation("Stab").Length * MILLISECONDS_IN_SECOND) );
		}

		public struct StabActionArgs {

			public StabActionArgs(PawnController _otherPawnController, float _damage, PawnController _ownerPawnController) 
			{
				otherPawnController = _otherPawnController;
				damage = _damage;
				ownerPawnController = _ownerPawnController;
			}

			public PawnController ownerPawnController;
			public PawnController otherPawnController;
			public float damage;
		}
	}
}