using System.Collections.Generic;
using System.Threading;
using System;
using Serilog;
using Pawn;
using Item;
using Interactable;
namespace Pawn.Action.Ability {
	public partial class StabAbility : IAbility {
		private PawnController? ownerPawnController;
		private PawnController? otherPawnController;
		public IItem? HeldItem {get; set;}
		//Stab action wont work like this but I need an object reference to the type
		public StabAbility() {}
		public StabAbility( PawnController _ownerPawnController, PawnController _otherPawnController){
			ownerPawnController = _ownerPawnController;
			otherPawnController = _otherPawnController;
			HeldItem = ownerPawnController.PawnInventory.GetWornEquipment(EquipmentType.HELD);
		}
		public IAbility Duplicate(PawnController _ownerPawnController, PawnController _otherPawnController) {
			return new StabAbility(_ownerPawnController, _otherPawnController);
		}
		public Boolean CanBeUsed(PawnController _ownerPawnController){
			//Stab abilty should pretty much always be valid to use
			return true;
		}
		private static int STAB_ACTION_COOLDOWN = 4000;
		private static float STAB_ACTION_MAX_RANGE = 2;
		public int CooldownMilliseconds {get {return STAB_ACTION_COOLDOWN;} }
		public string Name {get {return this.GetType().Name;}}
		public float MaxRange {get {return STAB_ACTION_MAX_RANGE;}}
		public AnimationName AnimationToPlay {get {return AnimationName.Stab;}}
		public void Execute() {
			//TODO: Stab should provide its own multipliers
			if(ownerPawnController == null || otherPawnController == null) {
				throw new NullReferenceException();
			}
			double damage = ownerPawnController.GetDamage();
			otherPawnController.TakeDamage(damage);
		}

		//A sign that this is now how I should be defining abilites
		public bool IsFinished() {
			throw new NotImplementedException();
		}

		public void Setup(IInteractable? target, IItem heldItem) {
			return;
		}

	}
}