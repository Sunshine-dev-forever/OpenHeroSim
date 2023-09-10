using System;
using Godot;

namespace Interactable
{
	//Represents anything that a pawn may want to interact with
	//This includes other pawns, harvestable resources, treasure chests, 
	//Shop tables (to buy things), shrines, etc
	// Another way of putting this is that only 
	public interface IInteractable
	{
		//the global position of the interactable
		public Transform3D GlobalTransform { get; set; }
		//if the object is queued to be free, this will return false
		// otherwise true
		public Boolean IsInstanceValid();
	}
}