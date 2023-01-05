using System.Collections.Generic;
using Pawn.Controller;
using Godot;
using System;

namespace Pawn.Action {
	public class Action : IAction
	{

		private static int NO_COOLDOWN = 0;
		private static float DEFAULT_RANGE = 2;
		private static bool DEFAULT_LOOPING = false;
		
		public Action(PawnController _ownerPawnController, System.Action _executable) {
			Tags = new List<ActionTags>();
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

		private bool isCurrentlyRunning = false;
		private DateTime timeStarted = DateTime.MinValue;
		public void execute() {
			executable();
			ownerPawnController.VisualController.SetAnimation(AnimationToPlay, loopAnimation);
			if(loopAnimation) {
				System.Threading.Thread.Sleep( animationPlayLengthMilliseconds );
			} else {
				System.Threading.Thread.Sleep( (int) ownerPawnController.VisualController.getAnimationLengthMilliseconds(AnimationToPlay) );
			}	
		}

		public System.Action executable {get; set;}

		public AnimationName AnimationToPlay {get; set;} = AnimationName.Interact;

		public bool IsFinished() {
			if(!isCurrentlyRunning) {
				throw new InvalidOperationException();
			}
			int timeRunningMilliseconds = (DateTime.Now - timeStarted).Milliseconds;
			if(loopAnimation) {
				return timeRunningMilliseconds > animationPlayLengthMilliseconds;
			} else {
				return timeRunningMilliseconds > ownerPawnController.VisualController.getAnimationLengthMilliseconds(AnimationToPlay);
			}
		}
	}
}