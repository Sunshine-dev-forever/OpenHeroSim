using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using Pawn.Tasks;
using Pawn;
using Pawn.Controller;

namespace UI {
	public class DebugUI : Spatial
	{
		//setup in ready
		private Camera camera = null!;
		private Label pawnNameValue = null!;
		private Label pawnTaskStatusValue = null!;
		private int RAY_LENGTH = 10000;
		public override void _Ready()
		{
			camera = GetNode<Camera>("Camera");	
			pawnNameValue = GetNode<Label>("GUI/VBoxContainer/NinePatchRect/PawnNameValue");
			pawnTaskStatusValue = GetNode<Label>("GUI/VBoxContainer/NinePatchRect/PawnStatusValue");
		}
		
		public override void _Input(InputEvent input) {
			if(input.IsActionPressed("mouse_left_click") && input is InputEventMouseButton) {
				CastRayFromCamera((InputEventMouseButton) input);
			} else if(input.IsActionPressed("ui_left")) {
				//pass
			}
		}

		private void CastRayFromCamera(InputEventMouseButton input) {
			//just a query should be fine to call outside of physics_process
			PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
			Vector3 from = camera.ProjectRayOrigin(input.Position);
			Vector3 to = from + camera.ProjectRayNormal(input.Position) * RAY_LENGTH;
			Godot.Collections.Dictionary result = spaceState.IntersectRay(from, to);
			//we really dont want to be using Godot dictionaries
			if(result.Count == 0) {
				//we hit nothing
				return;
			}
			if(result["collider"] is PawnRigidBody) {
				PawnController pawnController = ((PawnRigidBody) result["collider"]).GetPawnController();
				ITask task = pawnController.GetTask();
				if(pawnController.IsDying) {
					pawnTaskStatusValue.Text = "Dying";
				} else {
					pawnTaskStatusValue.Text = "Alive";
				}
				pawnNameValue.Text = pawnController.PawnInformation.Name;
			}

		}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//	  
	//  }
	}
}