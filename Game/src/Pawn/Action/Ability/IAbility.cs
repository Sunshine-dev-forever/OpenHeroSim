using Interactable;

namespace Pawn.Action.Ability;

public interface IAbility : IAction {
    // List<AbilityTags> Tags {get;}
    public bool CanBeUsed(IPawnController ownerPawnController);
    int CooldownMilliseconds { get; }
    void Setup(IInteractable? target);
}

// Tags are not needed right now but I am sure I will need them in the near future
// public enum AbilityTags {NONE}
