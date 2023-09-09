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

namespace UI.DebugInspector.Components
{
	public class TestDisplayable : IDisplay
	{

		private List<IDisplay> children = new List<IDisplay>();
		private List<string> details = new List<string>();

		public string Name { get; set; }

		private TestDisplayable()
		{
			Name = "";
		}

		public static TestDisplayable GenerateTest()
		{
			//Holy crap I need real unit tests
			TestDisplayable root = new TestDisplayable();
			TestDisplayable child1 = new TestDisplayable();
			TestDisplayable child2 = new TestDisplayable();
			root.AddDetail("name: rock");
			root.AddDetail("another detail");
			root.Name = "TesyMcTesterson";
			child1.Name = "stats";
			child1.AddDetail("attack: 32");
			child1.AddDetail("defense: 23213");
			child2.AddDetail("crappy wooden sword");
			child2.AddDetail("better wooden sheild");
			child2.Name = "equipment";
			root.AddChildDisplayable(child1);
			root.AddChildDisplayable(child2);
			return root;
		}

		private void AddChildDisplayable(IDisplay displayable)
		{
			children.Add(displayable);
		}
		private void AddDetail(string detail)
		{
			details.Add(detail);
		}

		public List<IDisplay> GetChildDisplays()
		{
			return children;
		}

		public List<string> GetDetails()
		{
			return details;
		}
	}
}
