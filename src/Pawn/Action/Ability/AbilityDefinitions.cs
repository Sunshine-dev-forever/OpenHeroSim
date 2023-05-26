using System.Collections.ObjectModel;
using System.Collections.Generic;
using Godot;
using Pawn;
using Pawn.Action.Ability;
using Item;
using System;
using Interactable;
using Serilog;
using Pawn.Targeting;

namespace Pawn.Action.Ability {
	public static class AbilityDefinitions {
		private static float DEFAULT_PROJECTILE_SPEED = 50;

		public const string STAB_ABILITY = "Stab ability";
		public const string THROW_ABILITY = "Throw ability";

		public static IAbility CreateStabAbility(PawnController ownerPawnController) {
			Predicate<PawnController> canBeUsed = (PawnController) => {return true;};

			System.Action<IInteractable?> abilityExecutable = (target) => {
				if(target == null) {
					throw new NullReferenceException("target was null");
				}
				PawnController otherPawnController = (PawnController) target;
				otherPawnController.TakeDamage(ownerPawnController.GetDamage());
			};

			IAbility ability = AbilityBuilder.Start(ownerPawnController, abilityExecutable, canBeUsed)
											.Animation(AnimationName.Stab)
											.MaxRange(2)
											.CooldownMilliseconds(2000)
											.Name(STAB_ABILITY)
											.Finish();
			return ability;
		}

		public static IAbility CreateThrowAbility(PawnController ownerPawnController) {
			//We need to have something to throw to use this ability
			Predicate<PawnController> canBeUsed = (PawnController) => {
				foreach (IItem item in ownerPawnController.PawnInventory.GetAllItemsInBag()) {
					if(item is Throwable) {
						if( ((Throwable) item).Count <= 0) {
							Log.Warning("There is a throwable with an ammo count of 0 in a pawn inventory");
						} else {
							return true;
						}
					}
				}
				return false;
			};

			System.Action<IInteractable?> abilityExecutable = (target) => {
				if(target == null) {
					throw new NullReferenceException("target was null");
				}
				PawnController otherPawnController = (PawnController) target;
				Throwable? itemToThrow = null;
				foreach (IItem item in ownerPawnController.PawnInventory.GetAllItemsInBag()) {
					if(item is Throwable) {
						if( ((Throwable) item).Count <= 0) {
							Log.Warning("There is a throwable with an ammo count of 0 in a pawn inventory");
						} else {
							itemToThrow = (Throwable) item;
						}
					}
				}

				if(itemToThrow == null) {
					Log.Error("Throw ability has nothing to throw!");
					return;
				}

				double damage = ownerPawnController.PawnInformation.BaseDamage + itemToThrow.Damage;
				//remove 1 ammo
				itemToThrow.Count -= 1;
				//remove item if it is out of ammo
				//TODO: does this cause memory leaks?
				if(itemToThrow.Count == 0) {
					ownerPawnController.PawnInventory.RemoveItem(itemToThrow);
				}
				
				Node3D mesh = (Node3D) itemToThrow.Mesh.Duplicate();

				otherPawnController.TakeDamage(damage);
				//also make a new projectile with the mesh in question
				CreateProjectile(mesh, ownerPawnController, otherPawnController);
			};

			IAbility ability = AbilityBuilder.Start(ownerPawnController, abilityExecutable, canBeUsed)
											.Animation(AnimationName.Interact)
											.MaxRange(10)
											.CooldownMilliseconds(5000)
											.Name(THROW_ABILITY)
											.Finish();
			return ability;
		}

		private static void CreateProjectile(Node3D mesh, PawnController ownerPawnController, PawnController otherPawnController) {
			//just needs to be one unit up, based off height of the pawn
			Vector3 offset = new Vector3(0,1,0);
			ITargeting target = new InteractableTargeting(otherPawnController, offset);
			Projectile projectile = new Projectile(mesh, target, DEFAULT_PROJECTILE_SPEED );
			ownerPawnController.GetParent().AddChild(projectile);
			projectile.GlobalTransform = new Transform3D(projectile.GlobalTransform.Basis, ownerPawnController.GlobalTransform.Origin + offset);
		}
	}
}