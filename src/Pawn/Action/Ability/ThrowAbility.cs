using System.Collections.Generic;
using System.Threading;
using System;
using Serilog;
using Pawn.Controller;
using Pawn.Item;
using Godot;
using Pawn.Targeting;

namespace Pawn.Action.Ability {
	public class ThrowAbility : IAbility {
		private static float DEFAULT_PROJECTILE_SPEED = 50;
		private PawnController? ownerPawnController;
		private PawnController? otherPawnController;
		//HeldItem has to be implemented THIS SUCKS SUCH ASS!
		//TODO: change this so that I have a Throwable object and that a held item is not required
		//TODO I am THROWING the item, I dont want to hold it
		public IItem? HeldItem {get; set;}
		private Throwable ItemToThrow;
		//Stab action wont work like this but I need an object reference to the type
		//why do I need an empty constructor??????
		public ThrowAbility() {}
		public ThrowAbility( PawnController _ownerPawnController, PawnController _otherPawnController){
			ownerPawnController = _ownerPawnController;
			otherPawnController = _otherPawnController;
			//TODO: implement an item count/amunition system of some kind
			foreach (IItem item in ownerPawnController.PawnInventory.GetAllItemsInBag()) {
				if(item is Throwable) {
					ItemToThrow = (Throwable) item;
				}
			}
		}
		public IAbility Duplicate(PawnController _ownerPawnController, PawnController _otherPawnController) {
			return new ThrowAbility(_ownerPawnController, _otherPawnController);
		}
		//todo: this needs  a unit (I know it is milliseconds for right now)
		//todo: both of these should not be prefaced with throw_
		private static int THROW_ACTION_COOLDOWN = 2000;
		private static float THROW_ACTION_MAX_RANGE = 20; // a very long range
		public int CooldownMilliseconds {get {return THROW_ACTION_COOLDOWN;} }
		public string Name {get {return this.GetType().Name;}}
		public float MaxRange {get {return THROW_ACTION_MAX_RANGE;}}
		private ActionTags[] tags = {ActionTags.COMBAT};
		public List<ActionTags> Tags {get {return new List<ActionTags>(tags);}}
		public AnimationName AnimationToPlay {get {return AnimationName.Interact;}}
		public void execute() {
			//TODO: throwing should provide its own multipliers
			if(ownerPawnController == null || otherPawnController == null) {
				throw new NullReferenceException();
			}
			double damage = ownerPawnController.PawnInformation.BaseDamage + ItemToThrow.Damage;
			//hopefully duplicate works as intended
			//TODO: actually remove ammo
			ownerPawnController.PawnInventory.RemoveItem(ItemToThrow);
			Spatial mesh = (Spatial) ItemToThrow.Mesh.Duplicate();

			otherPawnController.TakeDamage(damage);
			//also make a new projectile with the mesh in question
			CreateProjectile(mesh, ownerPawnController, otherPawnController);
		}

		public Boolean CanBeUsed(PawnController ownerPawnController, PawnController otherPawnController) {
			foreach (IItem item in ownerPawnController.PawnInventory.GetAllItemsInBag()) {
				if(item is Throwable) {
					return true;
				}
			}
			return false;
		}

		private void CreateProjectile(Spatial mesh, PawnController ownerPawnController, PawnController otherPawnController) {
			//just needs to be one unit up, based off height of the pawn
			Vector3 offset = new Vector3(0,1,0);
			ITargeting target = new InteractableTargeting(otherPawnController, offset);
			Projectile projectile = new Projectile(mesh, target, DEFAULT_PROJECTILE_SPEED );
			ownerPawnController.GetParent().AddChild(projectile);
			projectile.GlobalTransform = new Transform(projectile.GlobalTransform.basis, ownerPawnController.GlobalTransform.origin + offset);
		}

		//A sign that this is now how I should be defining abilites
		//Basically some interface fuckery is going on here
		public bool IsFinished() {
			throw new NotImplementedException();
		}

	}
}