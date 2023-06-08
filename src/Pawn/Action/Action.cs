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
		private static bool DEFAULT_LOOPING = false;
		private IPawnController ownerPawnController;
		public Action(IPawnController _ownerPawnController, System.Action _executable) {
			ownerPawnController = _ownerPawnController;
			executable = _executable;
		}

		private int animationPlayLengthMilliseconds = -1;

		//sets looping to true
		public void SetAnimationPlayLength(int milliseconds) {
			animationPlayLengthMilliseconds = milliseconds;
			loopAnimation = true;
		}
		public  bool loopAnimation {get; private set;} = DEFAULT_LOOPING;

		public int CooldownMilliseconds {get; set;} = NO_COOLDOWN;

		public string Name {get; set;} = "Generic Action";

		public float MaxRange {get; set;} = DEFAULT_RANGE;

		public IItem? HeldItem {get; set;} = null;
		private bool isCurrentlyRunning = false;
		private DateTime timeStarted = DateTime.MinValue;
		public void Execute() {
			if(isCurrentlyRunning) {
				Log.Error("Attempted to start the same action twice");
				Log.Error(System.Environment.StackTrace);
			}
			executable();
			ownerPawnController.PawnVisuals.SetAnimation(AnimationToPlay, loopAnimation);
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
			if(loopAnimation) {
				return timeRunningMilliseconds > animationPlayLengthMilliseconds;
			} else {
				return timeRunningMilliseconds > ownerPawnController.PawnVisuals.getAnimationLengthMilliseconds(AnimationToPlay);
			}
		}
	}
}