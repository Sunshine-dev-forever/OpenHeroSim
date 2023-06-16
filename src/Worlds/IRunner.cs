using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using System.Threading.Tasks;
using Pawn;
using Pawn.Goal;
using Item;
using Pawn.Action.Ability;
using Util;
using Pawn.Targeting;
using Interactable;

namespace Worlds 
{
	public interface IRunner
	{
		public KdTreeController KdTreeController {get;}
	}
}
