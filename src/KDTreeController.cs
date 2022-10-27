using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Serilog;
using KdTree;
using KdTree.Math;
using System.Linq;

public class KdTreeController : Node
{
	private KdTree<float, PawnController> pawnKdTree = new KdTree<float, PawnController>(3, new FloatMath());
	private List<PawnController> allPawnsList = new List<PawnController>();

    public override void _Ready()
    {
		//Log.Information("KD tree started!");
    }

	public override void _Process(float delta)
	{
		//Log.Information("KD tree Running!");
		//casually rebuild the entire tree
		//I should be able to multithread this
		KdTree<float, PawnController> newTree = new KdTree<float, PawnController>(3, new FloatMath());
		//Start from the end since we will be removing items
		for (int i = allPawnsList.Count - 1; i >= 0; i-- ) {
			PawnController pawnController = allPawnsList[i];
			if(!IsInstanceValid(pawnController)) {
				allPawnsList.RemoveAt(i);
				continue;
			}
			Vector3 location = pawnController.GlobalTransform.origin;
			newTree.Add(new[] {location.x, location.y, location.z}, pawnController );
		}
		pawnKdTree = newTree;
	}

	public void AddPawnToAllPawnList(PawnController pawnController) {
		allPawnsList.Add(pawnController);
	}

	public List<PawnController> GetNearestPawns(Vector3 location, int count) {
		IEnumerable<KdTreeNode<float, PawnController>> nearestNodes = 
			pawnKdTree.GetNearestNeighbours(new[] {location.x, location.y, location.z}, count);
		return nearestNodes.Select( (kdTreeNode) => (kdTreeNode.Value)).ToList();
	}
}
