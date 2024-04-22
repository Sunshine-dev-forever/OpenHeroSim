using Godot;
using Serilog;

namespace Util;

// a wrapper for the Godot resource loaders
// mainly just adds load defaults and error handling
public static class CustomResourceLoader {

    // attempts to load a mesh
    // if it fails, will return the default mesh
    public static Node3D LoadMesh(string filePath) {
        PackedScene? packedScene = GD.Load<PackedScene>(filePath);

        if (packedScene != null) {
            // loading went through fine, go next
            return (Node3D)packedScene.Instantiate();
        }
        else {
            Log.Error(typeof(CustomResourceLoader) +
                ": Failed to load mesh at " + filePath);

            return GetDefaultMesh();
        }
    }

    public static Control LoadUI(string filePath) {
        PackedScene? packedScene = GD.Load<PackedScene>(filePath);

        if (packedScene != null) {
            // loading went through fine, go next
            return (Control)packedScene.Instantiate();
        }
        else {
            Log.Error(typeof(CustomResourceLoader) +
                ": Failed to load UI at " + filePath);

            return GetDefaultUI();
        }
    }

    static Node3D GetDefaultMesh() {
        PackedScene? packedScene = GD.Load<PackedScene>(ResourcePaths.DEFAULT_MESH);

        if (packedScene != null) {
            return (Node3D)packedScene.Instantiate();
        }
        else {
            Log.Error(typeof(CustomResourceLoader)
                + ": Default mesh failed to load! Default mesh path: "
                + ResourcePaths.DEFAULT_MESH);

            return new Node3D();
        }
    }

    static Control GetDefaultUI() {
        PackedScene? packedScene = GD.Load<PackedScene>(ResourcePaths.DEFAULT_MESH);

        if (packedScene != null) {
            return (Control)packedScene.Instantiate();
        }
        else {
            Log.Error(typeof(CustomResourceLoader)
                + ": Default mesh failed to load! Default mesh path: "
                + ResourcePaths.DEFAULT_MESH);

            return new Control();
        }
    }
}
