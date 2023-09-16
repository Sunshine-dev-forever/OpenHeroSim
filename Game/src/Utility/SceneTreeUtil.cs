
using Godot;
namespace Util;
//a collection of fucntions that help with interacting with the godot scene tree
public static class SceneTreeUtil
{
    //removes all children from the given node
    public static void RemoveAllChildren(Node root)
    {
        foreach (Node node in root.GetChildren())
        {
            root.RemoveChild(node);
        }
    }

    public static void RemoveAndFreeAllChildren(Node root)
    {
        foreach (Node node in root.GetChildren())
        {
            root.RemoveChild(node);
            node.QueueFree();
        }
    }

    //Removes a child from its parent without affecting the parent
    //If the parent does not exist this will not throw an error
    public static void OrphanChild(Node node)
    {
        if (node.GetParent() != null)
        {
            node.GetParent().RemoveChild(node);
        }
    }
}