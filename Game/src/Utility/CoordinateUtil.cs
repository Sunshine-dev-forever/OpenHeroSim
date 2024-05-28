using System;
using Godot;
using Serilog;

namespace Util;

// a collection of fucntions that help with interacting with the godot scene tree
public static class CoordinateUtil {
    public static Vector3 GetRandomLocationWithinPlane(MeshInstance3D meshInstance3D) {
        if (meshInstance3D.Mesh is PlaneMesh planeMesh) {
            Vector3 origin = meshInstance3D.GlobalTransform.Origin;

            Random ran = new();

            double min_x = origin.X - (planeMesh.Size.X / 2);
            double max_x = origin.X + (planeMesh.Size.X / 2);
            double x = min_x + ((max_x - min_x) * ran.NextDouble());

            // Since a Plane is 2D, the 'Y' of the 2D plane is the 'Z' of 3D
            double min_z = origin.Z - (planeMesh.Size.Y / 2);
            double max_z = origin.Z + (planeMesh.Size.Y / 2);
            double z = min_z + ((max_z - min_z) * ran.NextDouble());
            
            return new Vector3((float) x, origin.Y, (float) z);
        }
        else {
            Log.Error("Mesh is not expected PlaneMesh in GetRandomLocationWithinPlane");
            return Vector3.Zero;
        }
    }
}
