using Godot;
using Interactable;

namespace Pawn.Targeting;
public class InteractableTargeting : ITargeting
{
    Vector3 offset;

    IInteractable Interactable { get; }
    public InteractableTargeting(IInteractable interactable)
    {
        Interactable = interactable;
    }
    public InteractableTargeting(IInteractable interactable, Vector3 _offset)
    {
        offset = _offset;
        Interactable = interactable;
    }
    public Vector3 GetTargetPosition()
    {
        return Interactable.GlobalTransform.Origin + offset;
    }
    public bool IsValid => Interactable.IsInstanceValid();
}