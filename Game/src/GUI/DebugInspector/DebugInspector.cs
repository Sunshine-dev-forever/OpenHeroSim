using System.Runtime.CompilerServices;
using System.Globalization;
using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using Pawn.Tasks;
using Pawn;
using Worlds;
using Interactable;
using Util;
using Item;

namespace UI
{
	public partial class DebugInspector : Control
	{
		private Control resizeableWindow = null!;
		private TreeController treeController = null!;

		public override void _Ready()
		{
			resizeableWindow = GetNode<Control>("ResizeableWindow");
			treeController = resizeableWindow.GetNode<TreeController>("TreeController");
			treeController.ItemSelected += (string input) => { GD.Print("recieved string: " + input); };
		}
	}
}
