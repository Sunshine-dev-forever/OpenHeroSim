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

	public void CreatePawn() {
		PackedScene pawnScene = GD.Load<PackedScene>("res://Objects/pawn.tscn");
		PawnController pawn = pawnScene.Instance<PawnController>();
		this.AddChild(pawn);
		pawn.Setup(kdTreeController);
		pawn.GlobalTransform = new Transform(pawn.GlobalTransform.basis, new Vector3(0,5,0));
		kdTreeController.AddPawnToAllPawnList(pawn);
	}

	public KdTreeController GetKdTreeController() {
		return kdTreeController;
	}
}
