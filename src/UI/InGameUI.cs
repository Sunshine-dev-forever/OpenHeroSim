using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using Pawn.Tasks;
using Pawn;
using Worlds;
using Interactable;
using Util;
using Item;

namespace UI {
	//this will get reworked!
	public partial class InGameUI : Node3D
	{
		//setup in ready
		private Camera3D camera = null!;
		private int RAY_LENGTH = 10000;

		private const uint STATIC_OBJECTS_MASK = 1;

		KdTreeController KdTreeController = null!;

		Control gui = null!;

		public override void _Ready()
		{
			camera = GetNode<Camera3D>("Camera3D");	
			IRunner runner = (IRunner) GetNode<Node>("Runner");
			KdTreeController = runner.KdTreeController;
			gui = GetNode<Control>("GUI");
		}
		
		public override void _Input(InputEvent input) {
			if(input.IsActionPressed("mouse_left_click") && input is InputEventMouseButton) {
				CastRayFromCamera((InputEventMouseButton) input);
			}
		}

		private void CastRayFromCamera(InputEventMouseButton input) {
			//just a query should be fine to call outside of physics_process
			PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
			Vector3 from = camera.ProjectRayOrigin(input.Position);
			Vector3 to = from + camera.ProjectRayNormal(input.Position) * RAY_LENGTH;
			//collistionMask of 1 should be only static objects with hitboxes
			uint collisionMask = STATIC_OBJECTS_MASK;
			PhysicsRayQueryParameters3D rayArgs = PhysicsRayQueryParameters3D.Create(from, to, collisionMask);

			Godot.Collections.Dictionary result = spaceState.IntersectRay(rayArgs);

			if(result.Count == 0) {
				//we hit nothing
				gui.Visible = false;
				return;
			} else {
				//we hit something
				//since our collisionmask is STATIC_OBJECTS_MASK, it had to be a static object of some kind
				//we just need the position
				Vector3 intersectionPosition = (Vector3) result["position"];
				List<IInteractable> interactables =  KdTreeController.GetNearestInteractables(intersectionPosition, 1);

				if(interactables.Count > 1) {
					Log.Error("InGameUI.cs: somehow got more than 1 interactable");
				} else if(interactables.Count == 0) {
					gui.Visible = false;
					return;
				}

				gui.Visible = true;
				VBoxContainer vBoxContainer = gui.GetNode<VBoxContainer>("VBoxContainer");
				SceneTreeUtil.RemoveAndFreeAllChildren(vBoxContainer);

				IInteractable target = interactables[0];

				if(target is IPawnController) {
					AddPawnInformation( (IPawnController) target);
				} else if(target is ItemContainer) {
					AddItemContainerInformation((ItemContainer) target);
				}
			}
		}

		private void AddItemContainerInformation(ItemContainer target) {
			VBoxContainer vBoxContainer = gui.GetNode<VBoxContainer>("VBoxContainer");
			Label titleLabel = new Label();
			titleLabel.Text = "Item Container";
			vBoxContainer.AddChild(titleLabel);

			vBoxContainer.AddChild(new HSeparator() );

			Label itemsLabel = new Label();
			itemsLabel.Text = "Items: \n";
			foreach (IItem item in target.Items) {
				itemsLabel.Text += item.Name + "\n";
			}

			vBoxContainer.AddChild(itemsLabel);
		}

		private void AddPawnInformation(IPawnController target) {
			VBoxContainer vBoxContainer = gui.GetNode<VBoxContainer>("VBoxContainer");
			Label titleLabel = new Label();
			titleLabel.Text = "IPawnController";
			vBoxContainer.AddChild(titleLabel);

			vBoxContainer.AddChild(new HSeparator() );

			Label itemsLabel = new Label();
			itemsLabel.Text = "Items: \n";
			foreach (IItem item in target.PawnInventory.GetAllItems()) {
				itemsLabel.Text += item.Name + "\n";
			}

			vBoxContainer.AddChild(itemsLabel);

		}
	}
}
