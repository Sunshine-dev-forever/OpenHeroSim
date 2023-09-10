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

namespace UI.DebugInspector.Display
{
	public interface IDisplayable
	{
		public IDisplay Display { get; }
	}
}
