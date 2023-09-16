using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using Pawn.Tasks;
using Pawn;
using Worlds.MainTest;
using Worlds.BattleRoyale;
using Interactable;
using Util;
using Item;
using GUI;
using GUI.DebugInspector;

namespace Worlds
{
	//this will get reworked!
	public partial class CSharpAdapter : Node3D
	{
		[Export]
		private RunnerType runnerType;
		[Export]
		private bool CreateFPSCounterUI;
		[Export]
		private bool CreateDebugInspector;
		private KdTreeController kdTreeController = null!;
		private IRunner runner = null!;
		public override void _Ready()
		{
			kdTreeController = new KdTreeController();
			//TODO: KDTree should not be a node, we can call Process on it from this class in all cases
			//not sure what to Export here to decide what Runner to load, but whatever
			//Right now just always create the mainTestRunner
			if (runnerType == RunnerType.MainTestRunner)
			{
				runner = new MainTestRunner(kdTreeController, this);
			}
			else
			{
				//TODO: update Battle Royale Runner
				runner = new BattleRoyaleRunner(kdTreeController, this);
			}
			Camera3D camera = this.GetNode<Camera3D>("Camera3D");

			if (CreateFPSCounterUI)
			{
				this.AddChild(CustomResourceLoader.LoadUI(ResourcePaths.FPS_COUNTER_UI));
			}
			if (CreateDebugInspector)
			{
				//Right now the UI element handles raycasting, which is a little ew. I would rather the C# adapter handle raycasting and pass that onto the UI elements
				DebugInspector DebugInspector = (DebugInspector)CustomResourceLoader.LoadUI(ResourcePaths.DEBUG_INSPECTOR_UI);
				this.AddChild(DebugInspector);
				DebugInspector.Setup(camera, kdTreeController);
			}
		}

		public override void _Process(double delta)
		{
			runner.Process(delta);
			kdTreeController.Process(delta);
			//call process on KDTree and all runners
		}

		public override void _Input(InputEvent input)
		{
			runner.Input(input);
			//not super sure how to handle input here, but whatever
		}

	}

	public enum RunnerType
	{
		MainTestRunner,
		BattleRoyaleRunner
	}
}
