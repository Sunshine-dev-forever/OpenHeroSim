using Godot;
using Serilog;

namespace Pawn;

// I might be able to get around having to use this class by referencing the
// KD-tree directly
public partial class PawnRigidBody : RigidBody3D
{
    [Export] NodePath PawnControllerPath = (NodePath) "..";
    public override void _Ready()
    {
        if (PawnControllerPath == null || PawnControllerPath == "")
        {
            Log.Error("YOU FORGOT TO SET PAWNCONTROLLER PATH IN PawnRigidBody.cs");
        }
    }

    public IPawnController GetPawnController()
    {
        return this.GetNode<IPawnController>(PawnControllerPath);
    }
}
