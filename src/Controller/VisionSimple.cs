using System;
using Serilog;
using System.Collections.Generic;
using Godot;

public class VisionSimple : Area
{
	[Export] NodePath pawnControllerPath;
	PawnController pawnController;

	GeneralUtil generalUtil = new GeneralUtil();

	[Signal] delegate void OnPawnEnterVision(PawnController pawnController);

	public override void _Ready() {
		generalUtil.Assert(pawnControllerPath != null, "pawnControllerPath was not initialized");
		pawnController = GetNode<PawnController>(pawnControllerPath);
	}
	//This seems to always grab the rigidbody, so no more work is needed for now
	public void _OnVisionBodyEntered(Node body) {
		if(body != pawnController){
			EmitSignal(nameof(OnPawnEnterVision), (PawnController)body);
		}
	}
}
