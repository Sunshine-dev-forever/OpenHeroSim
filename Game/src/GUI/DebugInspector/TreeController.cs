using Godot;
using System;
using Microsoft.CodeAnalysis;

namespace UI
{
	public delegate void ItemSelected(string itemname);

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
			GD.Print("Here is the meta data:" + selection.GetMetadata(0));
			ItemSelected?.Invoke((string)selection.GetMetadata(0));
		}


	}
}
