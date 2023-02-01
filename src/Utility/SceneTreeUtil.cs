
using Godot;
namespace Util {
	public static class SceneTreeUtil {
		public static void RemoveAllChildren(Node root) {
			foreach(Node node in root.GetChildren()) {
				root.RemoveChild(node);
			}
		}

		//Removes a child from its parent without affecting the parent
		public static void OrphanChild(Node node) {
			if(node.GetParent() != null) {
				node.GetParent().RemoveChild(node);
			}
		}

		// public static void PrintAllChildren(){
			//TODO
		// }
	}
}