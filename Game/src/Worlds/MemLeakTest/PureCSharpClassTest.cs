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

namespace Worlds.MemLeakTest 
{
	//A small class created for testing memory leaks in C#
	public class PureCSharpClassTest
	{
		public PureCSharpClassTest() {
			Log.Information("PureCSharpClassTest created!");
		}
		~PureCSharpClassTest() {
			Log.Information("PureCSharpClassTest finalizer called!");
		}
	}
}
