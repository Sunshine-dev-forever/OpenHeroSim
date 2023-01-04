using System.Runtime.CompilerServices;
using System.Globalization;
using Godot;
using System;
using Serilog;
using Pawn.Tasks;
using Pawn;
using System.Collections.Generic;
using Pawn.Item;

public class ItemContainer : Spatial, IInteractable {

	public ItemContainer(IItem _containedItem, Spatial _mesh) {
		ContainedItem = _containedItem;
		Mesh = _mesh;
		this.AddChild(Mesh);
	}
	public bool IsInstanceValid() {
		return IsInstanceValid(this);
	}
	public new void QueueFree() {
		base.QueueFree();
		if(ContainedItem != null){
			ContainedItem.QueueFree();
		}
	}
	public IItem ContainedItem;
	public Spatial Mesh;
}