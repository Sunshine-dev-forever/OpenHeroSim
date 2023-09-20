using Godot;
using System.Collections.Generic;

namespace Util;

public static class DebugOrb
{
    static readonly Dictionary<int, Node3D> orbs = new();

    public static void PlaceDebugOrb(Vector3 globalLocation, int id, Node anyNodeInSceneTree)
    {
        // if the orb has not been created yet then add it
        Node3D orb;

        if (orbs.ContainsKey(id))
        {
            orb = orbs[id];
        }
        else
        {
            Node parent = anyNodeInSceneTree.GetNode<Node>("/root/Node3D");
            orb = CustomResourceLoader.LoadMesh(ResourcePaths.DEFAULT_MESH);
            parent.AddChild(orb);
            orbs.Add(id, orb);
        }

        orb.GlobalPosition = globalLocation;
    }
}
