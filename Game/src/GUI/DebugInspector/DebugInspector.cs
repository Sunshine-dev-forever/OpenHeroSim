using Godot;
using Interactable;
using Serilog;
using System.Collections.Generic;

namespace GUI.DebugInspector;

public partial class DebugInspector : Control
{
    Control resizeableWindow = null!;
    DebugInspectorTree debugInspectorTree = null!;
    DebugInspectorDetails debugInspectorDetails = null!;
    Camera3D camera;
    KdTreeController kdTreeController;
    const int RAY_LENGTH = 10000;
    const uint STATIC_OBJECTS_MASK = 1;

    public override void _Ready()
    {
        resizeableWindow = GetNode<Control>("ResizeableWindow");
        debugInspectorTree = resizeableWindow.GetNode<DebugInspectorTree>("DebugInspectorTree");
        debugInspectorTree.ItemSelected += HandleItemSelected;
        debugInspectorDetails = resizeableWindow.GetNode<DebugInspectorDetails>("DebugInspectorDetails");
    }

    void HandleItemSelected(List<string> input)
    {
        debugInspectorDetails.ResetDetails();
        debugInspectorDetails.AddDisplay(input);
    }

    public void Setup(Camera3D _camera, KdTreeController _kdTreeController)
    {
        camera = _camera;
        kdTreeController = _kdTreeController;
    }

    public override void _Input(InputEvent input)
    {
        if (input.IsActionPressed("mouse_left_click") && input is InputEventMouseButton)
        {
            InputEventMouseButton mouseInput = (InputEventMouseButton)input;
            if (!ClickIsInsideThisUI(mouseInput))
            {
                CastRayFromCamera(mouseInput);
            }
        }

    }

    bool ClickIsInsideThisUI(InputEventMouseButton input)
    {
        InputEventMouseButton inputLocal = (InputEventMouseButton)resizeableWindow.MakeInputLocal(input);
        //the size of this control node is the entire screen, the resizeable window has the size we are looking for
        Rect2 uiSize = new(Vector2.Zero, resizeableWindow.Size);
        return uiSize.HasPoint(inputLocal.Position);
    }

    void CastRayFromCamera(InputEventMouseButton input)
    {
        //just a query should be fine to call outside of physics_process
        PhysicsDirectSpaceState3D spaceState = camera.GetWorld3D().DirectSpaceState;
        Vector3 from = camera.ProjectRayOrigin(input.Position);
        Vector3 to = from + camera.ProjectRayNormal(input.Position) * RAY_LENGTH;
        //collistionMask of 1 should be only static objects with hitboxes
        uint collisionMask = STATIC_OBJECTS_MASK;
        PhysicsRayQueryParameters3D rayArgs = PhysicsRayQueryParameters3D.Create(from, to, collisionMask);

        Godot.Collections.Dictionary result = spaceState.IntersectRay(rayArgs);

        if (result.Count == 0)
        {
            //we hit nothing
            this.Visible = false;
            return;
        }
        else
        {
            //we hit something
            //since our collisionmask is STATIC_OBJECTS_MASK, it had to be a static object of some kind
            //we just need the position
            Vector3 intersectionPosition = (Vector3)result["position"];
            List<IInteractable> interactables = kdTreeController.GetNearestInteractables(intersectionPosition, 1);

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
            IInteractable interactable = interactables[0];
            debugInspectorTree.CreateNewTree(interactable.Display);
        }
    }
}
