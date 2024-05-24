using Godot;
using Interactable;
using Item;
using Pawn;
using Pawn.Goal;
using Serilog;
using System;
using System.Collections.Generic;
using Util;
using GUI.DebugInspector;
using System.Diagnostics;

namespace Worlds.GoblinAttack;

public partial class GoblinAttackRunner : Node {
	readonly List<IPawnController> pawns = new();
	KdTreeController kdTreeController = null!;
	PawnGenerator pawnGenerator = null!;

	public override void _Ready() {
		kdTreeController = new KdTreeController();

		//setting up UI elements:
		AddChild(CustomResourceLoader.LoadUI(ResourcePaths.FPS_COUNTER_UI));
		Camera3D camera = GetNode<Camera3D>("Camera3D");

		DebugInspector DebugInspector = (DebugInspector)CustomResourceLoader.LoadUI(ResourcePaths.DEBUG_INSPECTOR_UI);
		AddChild(DebugInspector);
		DebugInspector.Setup(camera, kdTreeController);

		NavigationRegion3D navigationRegion3D = GetNode<NavigationRegion3D>("NavigationRegion3D");

		pawnGenerator = new PawnGenerator(
			this,
			kdTreeController,
			navigationRegion3D);

		List<IPawnGoal> pawnGoals = new() {
			new DebugGoal()
		};

		Spawner warriorGuild = GetNode<Spawner>("NavigationRegion3D/Buildings/WarriorsGuild");
		kdTreeController.AddInteractable(warriorGuild);
		warriorGuild.Setup(pawnGenerator, pawnGoals);
	}

	public override void _Input(InputEvent input) {
		if (input.IsActionPressed("mouse_left_click")) {
			// Do nothing... for now
		}
		else if (input.IsActionPressed("ui_left")) {

		}
		else if (input.IsActionPressed("ui_right")) {

		}
		else if (input.IsActionPressed("ui_up")) {

		}
	}

	public override void _Process(double delta) =>
		//I have a comment
		kdTreeController.Process();

	Vector3 GetRandomLocationInArena() {
		Random rand = new();
		int x = rand.Next(-249, 249);
		int z = rand.Next(-249, 249);
		int HEIGHT_DEFAULT = 4;
		return new Vector3(x, HEIGHT_DEFAULT, z);
	}

	// IPawnController CreatePawn(Vector3 location) {
	//     List<IPawnGoal> pawnGoals = new() {
	//         new HealGoal(),
	//         new DefendSelfGoal(),
	//         new LootGoal(),
	//         new ShopGoal(),
	//         new BattleRoyaleWanderGoal()
	//     };

	//     return pawnGenerator.RandomPawn(pawnGoals, location);
	// }

}
