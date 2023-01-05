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
	public List<IItem> Items;
	public Spatial Mesh;

	public ItemContainer(List<IItem> _items, Spatial _mesh) {
		Items = _items;
		Mesh = _mesh;
		this.AddChild(Mesh);
	}
	public bool IsInstanceValid() {
		return IsInstanceValid(this);
	}
	public new void QueueFree() {
		base.QueueFree();
		foreach(IItem item in Items) {
			item.QueueFree();
		}
	}
}