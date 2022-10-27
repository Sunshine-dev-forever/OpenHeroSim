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
		Spatial pawn = pawnScene.Instance<Spatial>();
		this.AddChild(pawn);
		pawn.GlobalTransform = new Transform(pawn.GlobalTransform.basis, new Vector3(0,5,0));
		kdTreeController.AddPawnToAllPawnList(pawn.GetChild<PawnController>(0));
	}

	public KdTreeController GetKdTreeController() {
		return kdTreeController;
	}
}
