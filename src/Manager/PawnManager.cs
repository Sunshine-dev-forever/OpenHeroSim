using Godot;

public class PawnManager : Node {

	KdTreeController kdTreeController = new KdTreeController();

	public override void _Ready()
	{
		this.AddChild(kdTreeController);
	}

	public override void _Input(InputEvent input) {
		if(input.IsActionPressed("mouse_left_click")) {
			//CreatePawn();
		}
	}

	public PawnController CreatePawn(Vector3 spawnLocation) {
		PackedScene pawnScene = GD.Load<PackedScene>("res://Objects/pawn.tscn");
		PawnController pawn = pawnScene.Instance<PawnController>();
		this.AddChild(pawn);
		pawn.Setup(kdTreeController);
		pawn.GlobalTransform = new Transform(pawn.GlobalTransform.basis, spawnLocation);
		kdTreeController.AddPawnToAllPawnList(pawn);
		return pawn;
	}

	public KdTreeController GetKdTreeController() {
		return kdTreeController;
	}
}
