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
	private DateTime TimeSinceLastEmpty = DateTime.MaxValue;
	private static int TIME_TO_LIVE_WHEN_EMPTY_SECONDS = 30;

	public override void _Process(float delta)
	{
		if(Items.Count == 0 && TimeSinceLastEmpty == DateTime.MaxValue) {
			TimeSinceLastEmpty = DateTime.Now;
		} else if (Items.Count >= 1) {
			TimeSinceLastEmpty = DateTime.MaxValue;
		}
		if( ((DateTime.Now - TimeSinceLastEmpty).TotalSeconds > TIME_TO_LIVE_WHEN_EMPTY_SECONDS )) {
			this.QueueFree();
		}
	}

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