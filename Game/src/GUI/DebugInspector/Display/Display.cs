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
using GUI.DebugInspector.Display;

namespace GUI.DebugInspector.Display
{
	public class Display : IDisplay
	{

		private List<IDisplay> children = new List<IDisplay>();
		private List<string> details = new List<string>();

		public string Name { get; set; }

		private Display()
		{
			Name = "";
		}

		public Display(string _name)
		{
			Name = _name;
		}

		public static Display GenerateTest()
		{
			//Holy crap I need real unit tests
			Display root = new Display();
			Display child1 = new Display();
			Display child2 = new Display();
			root.AddDetail("name: rock");
			root.AddDetail("another detail");
			root.Name = "TesyMcTesterson";
			child1.Name = "stats";
			child1.AddDetail("attack: 32");
			child1.AddDetail("defense: 23213");
			child2.AddDetail("crappy wooden sword");
			child2.AddDetail("better wooden sheild");
			child2.Name = "equipment";
			root.AddChildDisplay(child1);
			root.AddChildDisplay(child2);
			return root;
		}

		private void AddChildDisplay(IDisplay displayable)
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
