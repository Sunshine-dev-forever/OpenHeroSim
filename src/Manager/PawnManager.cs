using Godot;

public class PawnFactory : Node {
	public override void _Input(InputEvent input) {
		 if(input.IsActionPressed("mouse_left_click")) {
			//CreatePawn();
		 }
	}

	private void CreatePawn() {
		PackedScene pawnScene = GD.Load<PackedScene>("res://Objects/pawn.tscn");
		Spatial pawn = pawnScene.Instance<Spatial>();
		this.AddChild(pawn);
		pawn.GlobalTransform = new Transform(pawn.GlobalTransform.basis, new Vector3(0,5,0));
	}
}
