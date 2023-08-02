using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Serilog;
using KdTree;
using KdTree.Math;
using System.Linq;
using Pawn;
using Interactable;

public partial class KdTreeController
{
	private KdTree<float, IInteractable> kdTree = new KdTree<float, IInteractable>(3, new FloatMath());
	private List<IInteractable> allInteractables = new List<IInteractable>();

	public void Process(double delta)
	{
		//this rebuilds the kd-tree every single frame
		//in the future this would be optimized to only rebuild the tree every nth frame
		//alternatively, I could program this process function to operate on its own thread
		KdTree<float, IInteractable> newTree = new KdTree<float, IInteractable>(3, new FloatMath());
		
		//Start from the end of the list since we will be removing items
		for (int i = allInteractables.Count - 1; i >= 0; i--)
		{
			IInteractable interactable = allInteractables[i];
			//prune invalid instances
			if (interactable == null || !interactable.IsInstanceValid())
			{
				allInteractables.RemoveAt(i);
				continue;
			}
			//add valid instances to new tree
			Vector3 location = interactable.GlobalTransform.Origin;
			newTree.Add(new[] { location.X, location.Y, location.Z }, interactable);
		}
		//replace old tree with new tree
		kdTree = newTree;
	}

	//adds an interactable to the list of interatables
	//NOTE: this does not add anything to the KDtree directly
	public void AddInteractable(IInteractable interactable)
	{
		allInteractables.Add(interactable);
	}

	//count is the max number of neightbors to pull, keep low for better preformance I guess?
	public List<IInteractable> GetNearestInteractables(Vector3 location, int count)
	{
		IEnumerable<KdTreeNode<float, IInteractable>> nearestNodes =
			kdTree.GetNearestNeighbours(new[] { location.X, location.Y, location.Z }, count);
		return nearestNodes.Select((kdTreeNode) => (kdTreeNode.Value)).ToList();
	}

	//count is the max number of neightbors to pull, keep low for better preformance I guess?
	public List<IInteractable> GetNearestInteractableToInteractable(IInteractable interactable, int count)
	{
		Vector3 location = interactable.GlobalTransform.Origin;
		List<IInteractable> rtn = GetNearestInteractables(location, count + 1);
		rtn.Remove(interactable);
		return rtn;
	}
}
