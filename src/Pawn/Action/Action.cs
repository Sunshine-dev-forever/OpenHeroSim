
using System;
using System.Collections.Generic;
using Pawn.Controller;
using Godot;

namespace Pawn.Action {
	public class Action : IAction
	{

		private static int NO_COOLDOWN = 0;
		private static float DEFAULT_RANGE = 2;
		private static bool DEFAULT_LOOPING = false;
		
		public Action(List<ActionTags> _tags, PawnController _ownerPawnController, System.Action _executable) {
			Tags = _tags;
			ownerPawnController = _ownerPawnController;
			executable = _executable;
		}

		private int animationPlayLengthMilliseconds = -1;

		//sets looping to true
		public void SetAnimationPlayLength(int milliseconds) {
			animationPlayLengthMilliseconds = milliseconds;
			loopAnimation = true;
		}

		private PawnController ownerPawnController;

		public  bool loopAnimation {get; private set;} = DEFAULT_LOOPING;

		public int CooldownMilliseconds {get; set;} = NO_COOLDOWN;

		public List<ActionTags> Tags {get; set;}

		public string Name {get; set;} = "Generic Action";

		public float MaxRange {get; set;} = DEFAULT_RANGE;

		public Spatial? HeldItemMesh {get; set;} = null;

		public void execute() {
			executable();
			ownerPawnController.VisualController.SetAnimation(AnimationToPlay, loopAnimation);
			if(loopAnimation) {
				System.Threading.Thread.Sleep( animationPlayLengthMilliseconds );
			} else {
				System.Threading.Thread.Sleep( (int) ownerPawnController.VisualController.getAnimationLengthMilliseconds(AnimationToPlay));
			}
			
		}

		public System.Action executable {get; set;}

		public AnimationName AnimationToPlay {get; set;} = AnimationName.Interact;

	}
}