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
using UI.DebugInspector.Display;

namespace UI.DebugInspector
{
	public partial class DebugInspector : Control
	{
		private Control resizeableWindow = null!;
		private DebugInspectorTree debugInspectorTree = null!;
		private DebugInspectorDetails debugInspectorDetails = null!;

		public override void _Ready()
		{
			resizeableWindow = GetNode<Control>("ResizeableWindow");
			debugInspectorTree = resizeableWindow.GetNode<DebugInspectorTree>("DebugInspectorTree");
			debugInspectorTree.ItemSelected += HandleItemSelected;
			debugInspectorDetails = resizeableWindow.GetNode<DebugInspectorDetails>("DebugInspectorDetails");


		}

		private void HandleItemSelected(List<string> input)
		{
			debugInspectorDetails.ResetDetails();
			debugInspectorDetails.AddDisplay(input);
		}

		public override void _Input(InputEvent input)
		{
			//Todo: have string constants for action names
			if (input.IsAction("ui_right"))
			{
				debugInspectorTree.CreateNewTree(Display.Display.GenerateTest());
			}
		}
	}
}
