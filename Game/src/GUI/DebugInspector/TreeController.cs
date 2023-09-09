using Godot;
using System;
using Microsoft.CodeAnalysis;
using UI.DebugInspector.Components;
using Util;
using System.Collections.Generic;

namespace UI.DebugInspector
{
	public delegate void ItemSelected(List<string> details);

	public partial class TreeController : Control
	{
		public event ItemSelected? ItemSelected;
		public Tree Tree { get; private set; } = null!;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()//asdf
		{
			Tree = GetNode<Tree>("Tree");
			TreeItem root = Tree.CreateItem();
			root.SetText(0, "ROOT ROOT");

			TreeItem child1 = root.CreateChild();
			child1.SetMetadata(0, "I am the good child!");
			child1.SetText(0, "Angels and shit!");

			TreeItem child2 = root.CreateChild();
			child2.SetMetadata(0, "I am the bad child");
			child2.SetText(0, "Demons and Such");
			//Column should always be 0 as far as I can tell

			Tree.CellSelected += HandleCellSelected;

		}

		private void HandleCellSelected()
		{
			TreeItem selection = Tree.GetSelected();
			GD.Print("You selected" + selection.GetText(0) + "\n");
			GodotWrapper<List<string>> wrapper = (GodotWrapper<List<string>>)selection.GetMetadata(0);
			ItemSelected?.Invoke(wrapper.value);
		}

		public void CreateNewTree(IDisplay display)
		{
			//TODO: I can only hold this does not cause memory leaks
			Tree.Clear();
			TreeItem root = Tree.CreateItem();

			ConvertDisplayToTreeItem(root, display);

		}

		private void ConvertDisplayToTreeItem(TreeItem item, IDisplay display)
		{
			item.SetText(0, display.Name);
			item.SetMetadata(0, new GodotWrapper<List<string>>(display.GetDetails()));
			foreach (IDisplay childDisplay in display.GetChildDisplays())
			{
				TreeItem childItem = item.CreateChild();
				ConvertDisplayToTreeItem(childItem, childDisplay);
			}
		}


	}
}
