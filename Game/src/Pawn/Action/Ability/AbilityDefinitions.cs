using Godot;
using Interactable;
using Item;
using Pawn.Targeting;
using Serilog;
using System;

namespace Pawn.Action.Ability;
public static class AbilityDefinitions
{
    static readonly float DEFAULT_PROJECTILE_SPEED = 50;

    public const string STAB_ABILITY = "Stab ability";
    public const string THROW_ABILITY = "Throw ability";

    public static IAbility CreateStabAbility(IPawnController ownerPawnController)
    {
        Predicate<IPawnController> canBeUsed = (PawnController) => {return true;};

        System.Action<IInteractable?> abilityExecutable = (target) =>
        {
            if(target == null || !target.IsInstanceValid())
            {
                //Target is no longer valid for some reason
                return;
            }
            IPawnController otherPawnController = (IPawnController) target;
            otherPawnController.TakeDamage(ownerPawnController.GetDamage());
        };

        IAbility ability = AbilityBuilder.Start(ownerPawnController, abilityExecutable, canBeUsed)
                                            .Animation(AnimationName.MeleeAttack)
                                            .MaxRange(2)
                                            .CooldownMilliseconds(2000)
                                            .Name(STAB_ABILITY)
                                            .Finish();
        return ability;
    }

    public static IAbility CreateThrowAbility(IPawnController ownerPawnController)
    {
        //We need to have something to throw to use this ability
        Predicate<IPawnController> canBeUsed = (PawnController) =>
        {
            foreach (IItem item in ownerPawnController.PawnInventory.GetAllItemsInBag())
            {
                if(item is Throwable)
                {
                    if( ((Throwable) item).Count <= 0)
                    {
                        Log.Warning("There is a throwable with an ammo count of 0 in a pawn inventory");
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        };

        System.Action<IInteractable?> abilityExecutable = (target) =>
        {
            if(target == null || !target.IsInstanceValid())
            {
                //Target is no longer valid for some reason
                return;
            }
            IPawnController otherPawnController = (IPawnController) target;
            Throwable? itemToThrow = null;
            foreach (IItem item in ownerPawnController.PawnInventory.GetAllItemsInBag())
            {
                if(item is Throwable)
                {
                    if( ((Throwable) item).Count <= 0)
                    {
                        Log.Warning("There is a throwable with an ammo count of 0 in a pawn inventory");
                    }
                    else
                    {
                        itemToThrow = (Throwable) item;
                    }
                }
            }

            if(itemToThrow == null)
            {
                Log.Error("Throw ability has nothing to throw!");
                return;
            }

            double damage = ownerPawnController.PawnInformation.BaseDamage + itemToThrow.Damage;
            //remove 1 ammo
            itemToThrow.Count -= 1;
            bool deleteMeshWhenDone = false;
            if(itemToThrow.Count == 0)
            {
                ownerPawnController.PawnInventory.RemoveItem(itemToThrow);
                deleteMeshWhenDone = true;
            }

            Node3D mesh = (Node3D) itemToThrow.Mesh;

            otherPawnController.TakeDamage(damage);
            //also make a new projectile with the mesh in question
            CreateProjectile(mesh, ownerPawnController, otherPawnController, deleteMeshWhenDone);
        };

        IAbility ability = AbilityBuilder.Start(ownerPawnController, abilityExecutable, canBeUsed)
                                            .Animation(AnimationName.Interact)
                                            .MaxRange(10)
                                            .CooldownMilliseconds(5000)
                                            .Name(THROW_ABILITY)
                                            .Finish();
        return ability;
    }

    static void CreateProjectile(Node3D mesh, IPawnController ownerPawnController, IPawnController otherPawnController, bool deleteMeshWhenDone)
    {
        //just needs to be one unit up, based off height of the pawn
        Vector3 offset = new(0,1,0);
        ITargeting target = new InteractableTargeting(otherPawnController, offset);
        Projectile projectile = new(mesh, target, DEFAULT_PROJECTILE_SPEED, deleteMeshWhenDone );
        ownerPawnController.GetRootNode().AddChild(projectile);
        projectile.GlobalTransform = new Transform3D(projectile.GlobalTransform.Basis, ownerPawnController.GlobalTransform.Origin + offset);
    }
}