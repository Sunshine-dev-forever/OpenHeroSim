using System;
using Godot;
using System.Collections.Generic;
using Pawn.Controller;

//Represents anything that a pawn may want to interact with
//This includes other pawns, harvestable resources, treasure chests, 
//Shop tables (to buy things), shrines, etc
public interface IInteractable {
	public Transform3D GlobalTransform {get; set;}
	public Boolean IsInstanceValid();


}

//The various kinds of interactables
public enum InteractableType {PAWN, ITEM_CONTAINER}