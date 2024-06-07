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
using NSubstitute.Routing.Handlers;

namespace Worlds.GoblinAttack;

public partial class GoblinAttackRunner : Node {
	readonly List<IPawnController> pawns = new();
	KdTreeController kdTreeController = null!;
	PawnGenerator pawnGenerator = null!;
	MeshInstance3D VillageArea = null!;

	WanderGoal GetWanderGoal(MeshInstance3D area) => new(() => (
		CoordinateUtil.GetRandomLocationWithinPlane(area)
	));

	public override void _Ready() {
		kdTreeController = new KdTreeController();
		VillageArea = GetNode<MeshInstance3D>("NavigationRegion3D/Areas/VillageArea");
		MeshInstance3D GoblinArea = GetNode<MeshInstance3D>("NavigationRegion3D/Areas/GoblinArea");

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

		// Ideally we would not have to set these up, but Godot can only export Varient-compatable types
		// see: https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_variant.html#doc-c-sharp-variant
		Spawner warriorGuild = GetNode<Spawner>("NavigationRegion3D/Areas/VillageArea/WarriorsGuild");
		kdTreeController.AddInteractable(warriorGuild);
		warriorGuild.Setup(pawnGenerator,
			new List<IPawnGoal>() { GetWanderGoal(VillageArea) },
			new List<PawnType>() { PawnType.WARRIOR }
		);

		Spawner goblinGuild = GetNode<Spawner>("NavigationRegion3D/Areas/GoblinArea/GoblinGuild");
		kdTreeController.AddInteractable(goblinGuild);
		goblinGuild.Setup(pawnGenerator,
			new List<IPawnGoal>() { GetWanderGoal(GoblinArea) },
			new List<PawnType>() { PawnType.GOBLIN }
		);
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
