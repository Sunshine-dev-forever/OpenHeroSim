
using Godot;
namespace Util {
	public static class SceneTreeUtil {
		public static void RemoveAllChildren(Node root) {
			foreach(Node node in root.GetChildren()) {
				root.RemoveChild(node);
			}
		}

		// public static void PrintAllChildren(){
			//TODO
		// }
	}
}