using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using System.Threading.Tasks;
using Pawn;
using Pawn.Controller;
using Pawn.Goal;

//TODO: I need more examples on this
[System.Diagnostics.CodeAnalysis.SuppressMessage("Non-nullable", "CA8618:must contain non-null value exiting constructor", Justification = "Not production code.")]
public class AdhocTest : Node
{

	private KdTreeController kdTreeController = null!; 
	public override void _Ready()
	{
		kdTreeController = new KdTreeController();
		this.AddChild(kdTreeController);
	}
	
	public override void _Input(InputEvent input) {
		 if(input.IsActionPressed("mouse_left_click")) {
			//Adhoc();
		 } else if(input.IsActionPressed("ui_left")) {
			Adhoc2();
		 }
	}
	private float TimeSinceLastPawnCreation = 0;
	public override void _Process(float delta)
	{
		TimeSinceLastPawnCreation += delta;
		if(TimeSinceLastPawnCreation > 5) {
			TimeSinceLastPawnCreation = 0;
			CreatePawn();
		}
		
	}

	private void CreatePawn(){
		Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
		
		Vector3 location = GenerateRandomVector();
		Random rand = new Random();
		int rng = rand.Next(0,3);
		if(rng == 1) {
			PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new WanderGoal())
								.Location(location)
								.Weapon(createBasicWeapon())
								.Finish();
		} else {
			PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new WanderGoal())
								.Location(location)
								.Finish();
		}
	}

	private Vector3 GenerateRandomVector() {
		Random rand = new Random();
		int rng = rand.Next(0,3);
		if(rng == 0) {
			return new Vector3(23,5,23);
		} else if (rng == 1) {
			return new Vector3(-23,5,23);
		} else if (rng == 2) {
			return new Vector3(-23, 5, -23);
		} else if (rng == 3) {
			return new Vector3(23, 5, -23);
		}
		return new Vector3();
	}

	private void Adhoc2(){
		createBasicWeapon();
	}

	private Weapon createBasicWeapon() {
		Spatial ironSword = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/iron_sword.tscn").Instance();
		return new Weapon(20, ironSword);
	}
}
