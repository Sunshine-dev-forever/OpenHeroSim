using Godot;
using System;

namespace Pawn.Targeting;
public class StaticPointTargeting : ITargeting {
    Vector3 Point { get; }
    Predicate<Vector3> PointTester { get; }
    public StaticPointTargeting(Vector3 _point) {
        Point = _point;
        // If no predicate is specified then the point is assumed to always be valid
        PointTester = (Vector3 point) => true;
    }

    public StaticPointTargeting(Vector3 _point, Predicate<Vector3> predicate) {
        Point = _point;
        PointTester = predicate;
    }
    public Vector3 GetTargetPosition() => Point;
    public bool IsValid => PointTester(Point);

}
