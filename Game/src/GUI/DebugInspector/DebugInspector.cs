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
using UI.DebugInspector.Components;

namespace UI.DebugInspector
{
	public partial class DebugInspector : Control
	{
		private Control resizeableWindow = null!;
		private TreeController treeController = null!;
		private InspectorDetailsController InspectorDetailsController = null!;

		public override void _Ready()
		{
			resizeableWindow = GetNode<Control>("ResizeableWindow");
			treeController = resizeableWindow.GetNode<TreeController>("TreeController");
			treeController.ItemSelected += HandleItemSelected;
			InspectorDetailsController = resizeableWindow.GetNode<InspectorDetailsController>("InspectorDetailsController");


		}

		private void HandleItemSelected(List<string> input)
		{
			InspectorDetailsController.ResetDetails();
			InspectorDetailsController.AddDisplay(input);
		}

		public override void _Input(InputEvent input)
		{
			//Todo: have string constants for action names
			if (input.IsAction("ui_right"))
			{
				treeController.CreateNewTree(TestDisplayable.GenerateTest());
			}
		}
	}
}
