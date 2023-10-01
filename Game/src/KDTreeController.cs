using Godot;
using Interactable;
using KdTree;
using KdTree.Math;
using System.Collections.Generic;
using System.Linq;

public class KdTreeController
{
    KdTree<float, IInteractable> kdTree = new(3, new FloatMath());
    readonly List<IInteractable> allInteractables = new();

    public void Process(double delta)
    {
        // This rebuilds the kd-tree every single frame. In the future
        // this would be optimized to only rebuild the tree every nth frame
        // alternatively, I could program this process function to operate
        // on its own thread.
        KdTree<float, IInteractable> newTree = new(3, new FloatMath());

        // Start from the end of the list since we will be removing items
        for (int i = allInteractables.Count - 1; i >= 0; i--)
        {
            IInteractable interactable = allInteractables[i];

            // Prune invalid instances
            if (interactable == null || !interactable.IsInstanceValid())
            {
                allInteractables.RemoveAt(i);
                continue;
            }

            // Add valid instances to new tree
            Vector3 location = interactable.GlobalTransform.Origin;
            newTree.Add(new[] { location.X, location.Y, location.Z }, interactable);
        }

        // replace old tree with new tree
        kdTree = newTree;
    }

    // adds an interactable to the list of interatables
    // NOTE: this does not add anything to the KDtree directly
    public void AddInteractable(IInteractable interactable)
    {
        allInteractables.Add(interactable);
    }

    // count is the max number of neightbors to pull, keep low for better preformance I guess?
    // TODO: I am pretty sure interactables are in the order nearest to farthest, but I should double check that
    public List<IInteractable> GetNearestInteractables(Vector3 location, int count)
    {
        IEnumerable<KdTreeNode<float, IInteractable>> nearestNodes =
            kdTree.GetNearestNeighbours(new[] { location.X, location.Y, location.Z }, count);

        return nearestNodes.Select((kdTreeNode) => kdTreeNode.Value).ToList();
    }

    // count is the max number of neightbors to pull, keep low for better preformance I guess?
    // Will not include the the interactable used as a reference
    public List<IInteractable> GetNearestInteractableToInteractable(IInteractable interactable, int count)
    {
        Vector3 location = interactable.GlobalTransform.Origin;
        List<IInteractable> rtn = GetNearestInteractables(location, count + 1);
        rtn.Remove(interactable);
        return rtn;
    }
}
