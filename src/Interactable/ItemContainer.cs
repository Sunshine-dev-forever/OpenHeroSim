using System.Runtime.CompilerServices;
using System.Globalization;
using Godot;
using System;
using Serilog;
using Pawn.Tasks;
using Pawn;
using System.Collections.Generic;
using Item;
namespace Interactable {
	public partial class ItemContainer : Node3D, IInteractable {
		public List<IItem> Items;
		public Node3D Mesh;
		private DateTime TimeSinceLastEmpty = DateTime.MaxValue;
		private static int TIME_TO_LIVE_WHEN_EMPTY_SECONDS = 5;

		public override void _Process(double delta)
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

		public ItemContainer(List<IItem> _items, Node3D _mesh) {
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
}