
using Godot;
using Pawn.Targeting;

//not sure where to put this class in the file structure
//The projectile class is just eye candy for now
public partial class Projectile : Node3D
{
    readonly Node3D mesh;
    readonly ITargeting target;
    readonly float speed;
    const float GOAL_DISTANCE = 2;
    bool deleteMeshWhenDone;
    public Projectile(Node3D _mesh, ITargeting _target, float _speed, bool _deleteMeshWhenDone)
    {
        mesh = _mesh;
        target = _target;
        speed = _speed;
        this.AddChild(mesh);
        //rotating the x by -90 degrees should point the projectile forward
        mesh.RotationDegrees = new Vector3(-90, 0, 0);
        deleteMeshWhenDone = _deleteMeshWhenDone;
    }
    public override void _Process(double delta)
    {
        if (!target.IsValid)
        {
            if (!deleteMeshWhenDone)
            {
                this.RemoveChild(mesh);
            }

            this.QueueFree();
            return;
        }
        //get next location and look at it
        Vector3 targetLocation = target.GetTargetPosition();
        this.LookAt(targetLocation, Vector3.Up);
        Vector3 locationDiff = targetLocation - this.GlobalTransform.Origin;

        //if we are within goal distance, delete self
        if (locationDiff.Length() < GOAL_DISTANCE)
        {
            if (!deleteMeshWhenDone)
            {
                this.RemoveChild(mesh);
            }

            this.QueueFree();
            return;
        }
        //move to new location
        Vector3 goalDirection = locationDiff.Normalized();
        Vector3 newLocation = this.GlobalTransform.Origin + (goalDirection * (float)(speed * delta));
        this.GlobalTransform = new Transform3D(this.GlobalTransform.Basis, newLocation);
    }
}
