using System.Collections.Generic;
using Pawn;
using Godot;
using System;
using Serilog;
using Item;
using Interactable;

namespace Pawn.Action.Ability {
	public class Ability : IAbility
	{
		private static int DEFAULT_COOLDOWN_MILLISECONDS = 2000;
		private static float DEFAULT_RANGE = 2;
		private static Animation.LoopModeEnum DEFAULT_LOOPING = Animation.LoopModeEnum.None;
		public System.Action<IInteractable?> abilityExecutable {get; set;}
		public Predicate<IPawnController> canBeUsedPredicate {get; set;}
		private IInteractable? target;
		//animation play length milliseconds is only used if looping is set to true
		private int animationPlayLengthMilliseconds = -1;
		public AnimationName AnimationToPlay {get; set;} = AnimationName.Interact;
		public  Animation.LoopModeEnum loopMode {get; private set;} = DEFAULT_LOOPING;
		private IPawnController ownerPawnController;
		public int CooldownMilliseconds {get; set;} = DEFAULT_COOLDOWN_MILLISECONDS;
		public string Name {get; set;} = "Generic ability";
		public float MaxRange {get; set;} = DEFAULT_RANGE;
		public IItem? HeldItem {get; set;} = null;
		private bool isCurrentlyRunning = false;
		private DateTime timeStarted = DateTime.MinValue;
		private DateTime lastTimeAbilityUsed = DateTime.MinValue;

		public Ability(IPawnController _ownerPawnController, System.Action<IInteractable?> _executable, Predicate<IPawnController> _canBeUsedPredicate) {
			ownerPawnController = _ownerPawnController;
			abilityExecutable = _executable;
			canBeUsedPredicate = _canBeUsedPredicate;
		}
		//I only set a animation play length for looping animations, so this sets looping to be true
		public void SetAnimationPlayLength(int milliseconds) {
			animationPlayLengthMilliseconds = milliseconds;
			loopMode = Animation.LoopModeEnum.Linear;
		}

		public void Execute() {
			if(isCurrentlyRunning) {
				Log.Error("Attempted to start the same action twice");
				Log.Error(System.Environment.StackTrace);
			}
			lastTimeAbilityUsed = DateTime.Now;
			abilityExecutable(target);
			ownerPawnController.PawnVisuals.SetAnimation(AnimationToPlay, loopMode);
			isCurrentlyRunning = true;
			timeStarted = DateTime.Now;
		}
		public bool IsFinished() {
			if(!isCurrentlyRunning) {
				throw new InvalidOperationException();
			}
			double timeRunningMilliseconds = (DateTime.Now - timeStarted).TotalMilliseconds;
			bool isFinished;
			if(loopMode != Animation.LoopModeEnum.None) {
				// there is some kind of looping going on, so we use the local animationPlayLengthMilliseconds, which can be longer than the original
				// animation length.
				isFinished = timeRunningMilliseconds > animationPlayLengthMilliseconds;
			} else {
				//there is no looping, so we can just use the original animation length
				isFinished =  timeRunningMilliseconds > ownerPawnController.PawnVisuals.getAnimationLengthMilliseconds(AnimationToPlay);
			}
			if(isFinished) {
				isCurrentlyRunning = false;
			}
			return isFinished;
		}

		public bool CanBeUsed(IPawnController ownerPawnController)
		{
			bool offCoolDown = (DateTime.Now - lastTimeAbilityUsed).TotalMilliseconds > CooldownMilliseconds;
			bool ret = offCoolDown && canBeUsedPredicate(ownerPawnController);
			return ret;
		}

		public void Setup(IInteractable? _target, IItem? _heldItem) {
			target = _target;
			HeldItem = _heldItem;
		}

	}
}