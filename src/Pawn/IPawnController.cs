using Godot;
using Interactable;
using System;

namespace Pawn
{
    public interface IPawnController : IInteractable
    {
        PawnInformation PawnInformation { get; }
        PawnInventory PawnInventory { get; }
        PawnBrain PawnBrain { get; }
        PawnMovement PawnMovement { get; }
		PawnVisuals PawnVisuals {get; }
		//Returns true if the pawn has taken fatal damage at some point and is now handling death-related tasks,
		//Otherwise returns false
        bool IsDying { get; }
		//negative damage is Ignored
        void TakeDamage(double damage);
		//negative healing is Ignored
        void TakeHealing(double amount);
		//Returns the damage this pawn is capable of outputting
        double GetDamage();
		//returns the root node in the scene tree of this pawn
		Node GetRootNode();
    }
}