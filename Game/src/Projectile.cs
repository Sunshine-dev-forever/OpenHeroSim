using Godot;
using Pawn.Targeting;

// not sure where to put this class in the file structure
// The projectile class is just eye candy for now
public partial class Projectile : Node3D {
    const float GOAL_DISTANCE = 2;

    bool deleteMeshWhenDone;

    readonly Node3D mesh;
    readonly ITargeting target;
    readonly float speed;

    public Projectile(Node3D _mesh, ITargeting _target, float _speed, bool _deleteMeshWhenDone) {
        mesh = _mesh;
        target = _target;
        speed = _speed;

        AddChild(mesh);

        // Rotating the x by -90 degrees should point the projectile forward
        mesh.RotationDegrees = new Vector3(-90, 0, 0);

        deleteMeshWhenDone = _deleteMeshWhenDone;
    }

    public override void _Process(double delta) {
        if (!target.IsValid) {
            if (!deleteMeshWhenDone)
                RemoveChild(mesh);

            QueueFree();
            return;
        }

        // Get next location and look at it
        Vector3 targetLocation = target.GetTargetPosition();

        LookAt(targetLocation, Vector3.Up);

        Vector3 locationDiff = targetLocation - GlobalTransform.Origin;

        // If we are within goal distance, delete self
        if (locationDiff.Length() < GOAL_DISTANCE) {
            if (!deleteMeshWhenDone)
                RemoveChild(mesh);

            QueueFree();
            return;
        }

        // Move to new location
        Vector3 goalDirection = locationDiff.Normalized();
        Vector3 newLocation =
            GlobalTransform.Origin + (goalDirection * (float)(speed * delta));

        GlobalTransform = new Transform3D(
            basis: GlobalTransform.Basis,
            origin: newLocation);
    }
}
