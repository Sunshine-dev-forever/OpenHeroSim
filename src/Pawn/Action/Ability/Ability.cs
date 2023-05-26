using System.Collections.Generic;
using Pawn;
using Godot;
using System;
using Serilog;
using Item;
using Interactable;

namespace Pawn.Action.Ability {
	public partial class Ability : IAbility
	{
		private static int DEFAULT_COOLDOWN_MILLISECONDS = 2000;
		private static float DEFAULT_RANGE = 2;
		private static bool DEFAULT_LOOPING = false;
		public System.Action<IInteractable?> abilityExecutable {get; set;}
		public Predicate<PawnController> canBeUsedPredicate {get; set;}
		private IInteractable? target;
		//animation play length milliseconds is only used if looping is set to true
		private int animationPlayLengthMilliseconds = -1;
		public AnimationName AnimationToPlay {get; set;} = AnimationName.Interact;
		public  bool loopAnimation {get; private set;} = DEFAULT_LOOPING;
		private PawnController ownerPawnController;
		public int CooldownMilliseconds {get; set;} = DEFAULT_COOLDOWN_MILLISECONDS;
		public string Name {get; set;} = "Generic ability";
		public float MaxRange {get; set;} = DEFAULT_RANGE;
		public IItem? HeldItem {get; set;} = null;
		private bool isCurrentlyRunning = false;
		private DateTime timeStarted = DateTime.MinValue;
		private DateTime lastTimeAbilityUsed = DateTime.MinValue;

		public Ability(PawnController _ownerPawnController, System.Action<IInteractable?> _executable, Predicate<PawnController> _canBeUsedPredicate) {
			ownerPawnController = _ownerPawnController;
			abilityExecutable = _executable;
			canBeUsedPredicate = _canBeUsedPredicate;
		}
		//sets looping to true
		public void SetAnimationPlayLength(int milliseconds) {
			animationPlayLengthMilliseconds = milliseconds;
			loopAnimation = true;
		}

		public void Execute() {
			if(isCurrentlyRunning) {
				Log.Error("Attempted to start the same action twice");
				Log.Error(System.Environment.StackTrace);
			}
			lastTimeAbilityUsed = DateTime.Now;
			abilityExecutable(target);
			ownerPawnController.PawnVisuals.SetAnimation(AnimationToPlay, loopAnimation);
			isCurrentlyRunning = true;
			timeStarted = DateTime.Now;
		}
		public bool IsFinished() {
			if(!isCurrentlyRunning) {
				throw new InvalidOperationException();
			}
			double timeRunningMilliseconds = (DateTime.Now - timeStarted).TotalMilliseconds;
			bool isFinished;
			if(loopAnimation) {
				 isFinished = timeRunningMilliseconds > animationPlayLengthMilliseconds;
			} else {
				isFinished =  timeRunningMilliseconds > ownerPawnController.PawnVisuals.getAnimationLengthMilliseconds(AnimationToPlay);
			}
			if(isFinished) {
				isCurrentlyRunning = false;
			}
			return isFinished;
		}

		public bool CanBeUsed(PawnController ownerPawnController)
		{
			bool offCoolDown = (DateTime.Now - lastTimeAbilityUsed).TotalMilliseconds > CooldownMilliseconds;
			bool ret = offCoolDown && canBeUsedPredicate(ownerPawnController);
			return ret;
		}

		public void Setup(IInteractable? _target, IItem _heldItem) {
			target = _target;
			HeldItem = _heldItem;
		}

	}
}