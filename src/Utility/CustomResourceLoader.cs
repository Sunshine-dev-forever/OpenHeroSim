
using Godot;
using Serilog;
namespace Util {
	public static class CustomResourceLoader {
		private static string DEFAULT_MESH_FILE_PATH = "res://scenes/visual_sphere.tscn";

		//This function has a default, wooohooo!
		public static Spatial LoadMesh(string filePath) {
			PackedScene? packedScene = GD.Load<PackedScene>(filePath);

			if(packedScene != null) {
				//loading went through fine, go next
				return (Spatial) packedScene.Instance();
			} else {
				Log.Error(typeof(CustomResourceLoader) + ": Failed to load mesh at " + filePath);
				return GetDefaultMesh();
			}
		}

		private static Spatial GetDefaultMesh() {
			PackedScene? packedScene = GD.Load<PackedScene>(DEFAULT_MESH_FILE_PATH);
			if(packedScene != null) {
				return (Spatial) packedScene.Instance();
			} else {
				Log.Error(typeof(CustomResourceLoader) + ": Default mesh failed to load! Default mesh path: " + DEFAULT_MESH_FILE_PATH);
				return new Spatial();
			}
		}
	}
}