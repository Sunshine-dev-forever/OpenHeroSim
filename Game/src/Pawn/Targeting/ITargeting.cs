using Godot;

namespace Pawn.Targeting;
public interface ITargeting
{
    public Vector3 GetTargetPosition();
    public bool IsValid { get; }
}