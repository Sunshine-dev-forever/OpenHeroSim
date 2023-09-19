using Godot;

namespace GUI.DebugInspector;

public partial class ResizeableWindow : Control
{
    bool isResizing = false;
    Vector2 lastMouseposition = Vector2.Zero;

    public override void _Input(InputEvent input)
    {
        // Todo: have string constants for action names
        if (Input.IsActionJustPressed("mouse_left_click"))
        {
            Vector2 localMousePosition = GetLocalMousePosition();
            // TODO: resize area should be a different color, but I dont care about that now
            if (localMousePosition.X is > 0 and < 20)
            {
                isResizing = true;
                lastMouseposition = localMousePosition;
            }
        }
        else if (Input.IsActionPressed("mouse_left_click") && isResizing)
        {
            this.OffsetLeft -= lastMouseposition.X - GetLocalMousePosition().X;
            lastMouseposition = GetLocalMousePosition();
        }
        else if (Input.IsActionJustReleased("mouse_left_click"))
        {
            isResizing = false;
        }
    }
}
