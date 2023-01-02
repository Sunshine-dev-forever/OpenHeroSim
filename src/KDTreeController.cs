using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Serilog;
using KdTree;
using KdTree.Math;
using System.Linq;
using Pawn.Controller;

public class KdTreeController : Node
{
	private KdTree<float, IInteractable> kdTree = new KdTree<float, IInteractable>(3, new FloatMath());
	private List<IInteractable> allInteractables = new List<IInteractable>();

    public override void _Ready()
    {
		//Log.Information("KD tree started!");
    }

	public override void _Process(float delta)
	{
		//casually rebuild the entire tree
		//I should be able to multithread this
		KdTree<float, IInteractable> newTree = new KdTree<float, IInteractable>(3, new FloatMath());
		//Start from the end since we will be removing items
		for (int i = allInteractables.Count - 1; i >= 0; i-- ) {
			IInteractable interactable = allInteractables[i];
			//prune invalid instances
			if(interactable == null || !interactable.IsInstanceValid()) {
				allInteractables.RemoveAt(i);
				continue;
			}
			//add valid instances to new tree
			Vector3 location = interactable.GlobalTransform.origin;
			newTree.Add(new[] {location.x, location.y, location.z}, interactable );
		}
		//replace old tree with new tree
		kdTree = newTree;
	}

	public void AddInteractable(IInteractable interactable) {
		allInteractables.Add(interactable);
	}

	//count is the max number of neightbors to pull, keep low for better preformance I guess?
	public List<IInteractable> GetNearestInteractables(Vector3 location, int count) {
		IEnumerable<KdTreeNode<float, IInteractable>> nearestNodes = 
			kdTree.GetNearestNeighbours(new[] {location.x, location.y, location.z}, count);
		return nearestNodes.Select( (kdTreeNode) => (kdTreeNode.Value)).ToList();
	}

	//count is the max number of neightbors to pull, keep low for better preformance I guess?
	public List<IInteractable> GetNearestInteractableToInteractable(IInteractable interactable, int count) {
		Vector3 location = interactable.GlobalTransform.origin;
		List<IInteractable> rtn = GetNearestInteractables(location, count);
		//This probablly wont throw an exception
		rtn.Remove(interactable);
		return rtn;
	}
}
