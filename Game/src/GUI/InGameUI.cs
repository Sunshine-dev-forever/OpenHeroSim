using Godot;
using Interactable;
using Item;
using Pawn;
using Serilog;
using System.Collections.Generic;
using Util;

namespace GUI;

// this will get reworked!
public partial class InGameUI : Control
{
    // setup in ready
    Camera3D camera = null!;
    const int RAY_LENGTH = 10000;
    const uint STATIC_OBJECTS_MASK = 1;
    KdTreeController KdTreeController = null!;

    public override void _Ready()
    {
    }

    public void Setup(Camera3D _camera, KdTreeController _KdTreeController)
    {
        camera = _camera;
        KdTreeController = _KdTreeController;
    }

    public override void _Input(InputEvent input)
    {
        if (input.IsActionPressed("mouse_left_click") && input is InputEventMouseButton)
        {
            CastRayFromCamera((InputEventMouseButton)input);
        }
    }

    void CastRayFromCamera(InputEventMouseButton input)
    {
        // just a query should be fine to call outside of physics_process
        PhysicsDirectSpaceState3D spaceState = camera.GetWorld3D().DirectSpaceState;
        Vector3 from = camera.ProjectRayOrigin(input.Position);
        Vector3 to = from + (camera.ProjectRayNormal(input.Position) * RAY_LENGTH);
        
        // collistionMask of 1 should be only static objects with hitboxes
        uint collisionMask = STATIC_OBJECTS_MASK;
        PhysicsRayQueryParameters3D rayArgs = PhysicsRayQueryParameters3D.Create(from, to, collisionMask);

        Godot.Collections.Dictionary result = spaceState.IntersectRay(rayArgs);

        if (result.Count == 0)
        {
            // we hit nothing
            this.Visible = false;
            return;
        }
        else
        {
            // we hit something
            // since our collisionmask is STATIC_OBJECTS_MASK, it had to be a static object of some kind
            // we just need the position
            Vector3 intersectionPosition = (Vector3)result["position"];
            List<IInteractable> interactables = KdTreeController.GetNearestInteractables(intersectionPosition, 1);

            if (interactables.Count > 1)
            {
                Log.Error("InGameUI.cs: somehow got more than 1 interactable");
            }
            else if (interactables.Count == 0)
            {
                this.Visible = false;
                return;
            }

            this.Visible = true;
            VBoxContainer vBoxContainer = this.GetNode<VBoxContainer>("VBoxContainer");
            SceneTreeUtil.RemoveAndFreeAllChildren(vBoxContainer);

            IInteractable target = interactables[0];

            if (target is IPawnController)
            {
                AddPawnInformation((IPawnController)target);
            }
            else if (target is ItemContainer)
            {
                AddItemContainerInformation((ItemContainer)target);
            }
        }
    }

    void AddItemContainerInformation(ItemContainer target)
    {
        VBoxContainer vBoxContainer = this.GetNode<VBoxContainer>("VBoxContainer");
        
        Label titleLabel = new()
        {
            Text = "Item Container"
        };
        vBoxContainer.AddChild(titleLabel);

        vBoxContainer.AddChild(new HSeparator());

        Label itemsLabel = new()
        {
            Text = "Items: \n"
        };
        foreach (IItem item in target.Items)
        {
            itemsLabel.Text += item.Name + "\n";
        }

        vBoxContainer.AddChild(itemsLabel);
    }

    void AddPawnInformation(IPawnController target)
    {
        VBoxContainer vBoxContainer = this.GetNode<VBoxContainer>("VBoxContainer");
        
        Label titleLabel = new()
        {
            Text = "IPawnController"
        };
        vBoxContainer.AddChild(titleLabel);

        vBoxContainer.AddChild(new HSeparator());

        Label equippedItemsLabel = new()
        {
            Text = "Equipped Items: \n"
        };
        
        foreach (IItem item in target.PawnInventory.GetAllEquippedItems())
        {
            equippedItemsLabel.Text += item.Name + "\n";
        }

        vBoxContainer.AddChild(equippedItemsLabel);
        vBoxContainer.AddChild(new HSeparator());
        
        Label bagItemsLabel = new()
        {
            Text = "Bagged Items: \n"
        };
        
        foreach (IItem item in target.PawnInventory.GetAllItemsInBag())
        {
            bagItemsLabel.Text += item.Name + "\n";
        }

        vBoxContainer.AddChild(bagItemsLabel);

    }
}
