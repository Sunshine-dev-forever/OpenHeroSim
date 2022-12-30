using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using System.Threading.Tasks;
using Pawn;
using Pawn.Controller;

//TODO: I need more examples on this
[System.Diagnostics.CodeAnalysis.SuppressMessage("Non-nullable", "CA8618:must contain non-null value exiting constructor", Justification = "Not production code.")]
public class AdhocTest : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	// Called when the node enters the scene tree for the first time.

	private KdTreeController kdTreeController = null!; 
	public override void _Ready()
	{
		//kdTreeController = GetNode<KdTreeController>("/root/KdTreeController");
	}
	
	public override void _Input(InputEvent input) {
		 if(input.IsActionPressed("mouse_left_click")) {
			//Adhoc();
		 } else if(input.IsActionPressed("ui_left")) {
			//Adhoc2();
		 }
	}
	private float TimeSinceLastPawnCreation = 0;
	public override void _Process(float delta)
	{
		TimeSinceLastPawnCreation += delta;
		if(TimeSinceLastPawnCreation > 5) {
			TimeSinceLastPawnCreation = 0;
			Adhoc();
		}
		
	}
	private PawnController testPawn = null!;
	private void Adhoc(){
		//PackedScene pawnScene = GD.Load<PackedScene>("res://Objects/pawn.tscn");
		//Spatial pawn = pawnScene.Instance<Spatial>();
		//Node navi = GetNode<Node>("/root/Spatial/Navigation");
		//this.AddChild(pawn);
		//pawn.GlobalTransform = new Transform(pawn.GlobalTransform.basis, new Vector3(5,5,0));
		//kdTreeController.AddPawnToAllPawnList(pawn);
		PawnManager pawnManager = GetNode<PawnManager>("/root/Spatial/Navigation");
		
		Vector3 location = new Vector3(0,0,0);
		Random rand = new Random();
		int rng = rand.Next(0,3);
		if(rng == 0) {
			location = new Vector3(23,5,23);
		} else if (rng == 1) {
			location = new Vector3(-23,5,23);
		} else if (rng == 2) {
			location = new Vector3(-23, 5, -23);
		} else if (rng == 3) {
			location = new Vector3(23, 5, -23);
		}
		testPawn = pawnManager.CreatePawn(location);


		//Log.Information("the current position is: " + pawn.GlobalTransform.origin);
	}

	private void Adhoc2(){
		testPawn.Adhoc();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//	  
//  }
}
