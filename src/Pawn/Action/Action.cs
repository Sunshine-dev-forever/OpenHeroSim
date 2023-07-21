using System.Collections.Generic;
using Pawn;
using Godot;
using System;
using Serilog;
using Item;
using Interactable;

namespace Pawn.Action {
	public class Action : IAction
	{
		private static int NO_COOLDOWN = 0;
		private static float DEFAULT_RANGE = 2;
		private static Animation.LoopModeEnum DEFAULT_LOOPING = Animation.LoopModeEnum.None;
		private IPawnController ownerPawnController;
		public Action(IPawnController _ownerPawnController, System.Action _executable) {
			ownerPawnController = _ownerPawnController;
			executable = _executable;
		}

		private int animationPlayLengthMilliseconds = -1;

		//sets looping to true
		public void SetAnimationPlayLength(int milliseconds) {
			animationPlayLengthMilliseconds = milliseconds;
			loopMode = Animation.LoopModeEnum.Linear;
		}
		public  Animation.LoopModeEnum loopMode {get; private set;} = DEFAULT_LOOPING;

		public int CooldownMilliseconds {get; set;} = NO_COOLDOWN;

		public string Name {get; set;} = "Generic Action";

		public float MaxRange {get; set;} = DEFAULT_RANGE;
		private bool isCurrentlyRunning = false;
		private DateTime timeStarted = DateTime.MinValue;
		public void Execute() {
			if(isCurrentlyRunning) {
				Log.Error("Attempted to start the same action twice");
				Log.Error(System.Environment.StackTrace);
			}
			executable();
			ownerPawnController.PawnVisuals.SetAnimation(AnimationToPlay, loopMode);
			isCurrentlyRunning = true;
			timeStarted = DateTime.Now;
		}
		public System.Action executable {get; set;}
		public AnimationName AnimationToPlay {get; set;} = AnimationName.Interact;
		public bool IsFinished() {
			if(!isCurrentlyRunning) {
				throw new InvalidOperationException();
			}
			//only gets milliseconds between 0 and 1000
			double timeRunningMilliseconds = (DateTime.Now - timeStarted).TotalMilliseconds;
			//todo: refactor this to the positive side (check for both looping possabilites)
			if(loopMode != Animation.LoopModeEnum.None) {
				// there is some kind of looping going on, so we use the local animationPlayLengthMilliseconds, which can be longer than the original
				// animation length.
				return timeRunningMilliseconds > animationPlayLengthMilliseconds;
			} else {
				//there is no looping, so we can just use the original animation length
				return timeRunningMilliseconds > ownerPawnController.PawnVisuals.getAnimationLengthMilliseconds(AnimationToPlay);
			}
		}
	}
}