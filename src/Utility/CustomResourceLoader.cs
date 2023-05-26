
using Godot;
using Serilog;
namespace Util {
	public static class CustomResourceLoader {


		//This function has a default, wooohooo!
		public static Node3D LoadMesh(string filePath) {
			PackedScene? packedScene = GD.Load<PackedScene>(filePath);

			if(packedScene != null) {
				//loading went through fine, go next
				return (Node3D) packedScene.Instantiate();
			} else {
				Log.Error(typeof(CustomResourceLoader) + ": Failed to load mesh at " + filePath);
				return GetDefaultMesh();
			}
		}

		private static Node3D GetDefaultMesh() {
			PackedScene? packedScene = GD.Load<PackedScene>(ResourcePaths.DEFAULT_MESH_FILE_PATH);
			if(packedScene != null) {
				return (Node3D) packedScene.Instantiate();
			} else {
				Log.Error(typeof(CustomResourceLoader) + ": Default mesh failed to load! Default mesh path: " + ResourcePaths.DEFAULT_MESH_FILE_PATH);
				return new Node3D();
			}
		}
	}
}