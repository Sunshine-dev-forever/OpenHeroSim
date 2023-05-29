using System.Collections.Generic;
using Godot;

namespace Util {
	public static class DebugOrb {
		private static Dictionary<int, Node3D> orbs = new Dictionary<int, Node3D>();
		public static void PlaceDebugOrb(Vector3 globalLocation, int id, Node anyNodeInSceneTree) {
			//if the orb has not been created yet then add it
			Node3D orb;
			if(orbs.ContainsKey(id)) {
				orb = orbs[id];
			} else {
				Node parent = anyNodeInSceneTree.GetNode<Node>("/root/Node3D");
				orb = CustomResourceLoader.LoadMesh(ResourcePaths.DEFAULT_MESH_FILE_PATH);
				parent.AddChild(orb);
				orbs.Add(id, orb);
			}
			orb.GlobalPosition = globalLocation;
		}
	}
}