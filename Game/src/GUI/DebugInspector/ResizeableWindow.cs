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
	public partial class ResizeableWindow : Control
	{
		private bool isResizing = false;
		private Vector2 lastMouseposition = Vector2.Zero;
		public override void _Input(InputEvent input)
		{
			//Todo: have string constants for action names
			if (Input.IsActionJustPressed("mouse_left_click"))
			{
				Vector2 localMousePosition = GetLocalMousePosition();
				//TODO: resize area should be a different color, but I dont care about that now
				if (0 < localMousePosition.X && localMousePosition.X < 20)
				{
					isResizing = true;
					lastMouseposition = localMousePosition;
				}
			}
			else if (Input.IsActionPressed("mouse_left_click") && isResizing)
			{
				this.OffsetLeft = this.OffsetLeft - (lastMouseposition.X - GetLocalMousePosition().X);
				lastMouseposition = GetLocalMousePosition();
			}
			else if (Input.IsActionJustReleased("mouse_left_click"))
			{
				isResizing = false;
			}
		}
	}
}
