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

namespace Worlds.BattleRoyale;

public partial class BattleRoyaleRunner : Node
{
	static readonly int NUMBER_OF_PAWNS_TO_SPAWN = 100;
	static readonly int NUMBER_OF_CHESTS_TO_SPAWN = 50;

	List<IPawnController> pawns = new();
	KdTreeController kdTreeController = null!;
	PawnGenerator pawnGenerator = null!;

	public override void _Ready()
	{
		kdTreeController = new KdTreeController();

		//setting up UI elements:
		this.AddChild(CustomResourceLoader.LoadUI(ResourcePaths.FPS_COUNTER_UI));
		Camera3D camera = this.GetNode<Camera3D>("Camera3D");

		DebugInspector DebugInspector = (DebugInspector)CustomResourceLoader.LoadUI(ResourcePaths.DEBUG_INSPECTOR_UI);
		this.AddChild(DebugInspector);
		DebugInspector.Setup(camera, kdTreeController);

		NavigationRegion3D navigationRegion3D = this.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");

		pawnGenerator = new PawnGenerator(
			this,
			kdTreeController,
			navigationRegion3D);
	}

	public override void _Input(InputEvent input)
	{
		if (input.IsActionPressed("mouse_left_click"))
		{
			// Do nothing... for now
		}
		else if (input.IsActionPressed("ui_left"))
		{
			// spawns pawns in random locations
			for (int x = 0; x < NUMBER_OF_PAWNS_TO_SPAWN; x++)
			{
				pawns.Add(CreatePawn(GetRandomLocationInArena()));
			}
		}
		else if (input.IsActionPressed("ui_right"))
		{
			for (int x = 0; x < NUMBER_OF_CHESTS_TO_SPAWN; x++)
			{
				CreateItemChest(GetRandomLocationInArena());
			}
		}
		else if (input.IsActionPressed("ui_up"))
		{
			FogController.GetFogController().StartFog();
		}
	}

	public override void _Process(double delta)
	{
		kdTreeController.Process(delta);

		// iterate through all pawns, deal damage those that are outside the bounds
		for (int i = pawns.Count - 1; i >= 0; i--)
		{
			IPawnController pawn = pawns[i];
			if (!Node.IsInstanceValid(pawn.GetRootNode()))
			{
				pawns.RemoveAt(i);
				break;
			}
			// TODO: do damage to pawn if they are outside bounds
		}

		UpdateBarriers();
	}

	Vector3 GetRandomLocationInArena()
	{
		Random rand = new();
		int x = rand.Next(-249, 249);
		int z = rand.Next(-249, 249);
		int HEIGHT_DEFAULT = 4;
		return new Vector3(x, HEIGHT_DEFAULT, z);
	}

	IPawnController CreatePawn(Vector3 location)
	{
		List<IPawnGoal> pawnGoals = new() {
			new HealGoal(),
			new DefendSelfGoal(),
			new LootGoal(),
			new BattleRoyaleWanderGoal()
		};

		return pawnGenerator.RandomPawn(pawnGoals, location);
	}

	void UpdateBarriers()
	{
		// TODO: a runner that needs to interact with the scene..... annoying
		// for now I can just insure that the blocks are direct children of the passed node storage
		// but that is not optimal
		Node3D NegX = this.GetNode<Node3D>("NegX");
		Node3D NegZ = this.GetNode<Node3D>("NegZ");
		Node3D PosX = this.GetNode<Node3D>("PosX");
		Node3D PosZ = this.GetNode<Node3D>("PosZ");

		float newDist = (float)FogController.GetFogController().GetFogPosition();

		SetOrigin(NegX, new Vector3(-newDist, 0, 0));
		SetOrigin(NegZ, new Vector3(0, 0, -newDist));
		SetOrigin(PosX, new Vector3(newDist, 0, 0));
		SetOrigin(PosZ, new Vector3(0, 0, newDist));
	}

	void SetOrigin(Node3D spatial, Vector3 origin)
	{
		spatial.GlobalTransform = new Transform3D(
			spatial.GlobalTransform.Basis,
			origin);
	}

	Equipment? GetRandomWeapon()
	{
		Random rand = new();
		int rng = rand.Next(0, 100);

		if (rng > 40)
			return null;

		return rng > 15 ?
			CreateRustedDagger() : rng > 4 ?
			CreateIronSword() : CreateLightSaber();
	}

	void CreateItemChest(Vector3 location)
	{
		// gonna override the height here
		// TODO: not sure if this is mutable
		location.Y = 0.5f;

		Node3D TreasureChestMesh =
			CustomResourceLoader.LoadMesh(ResourcePaths.TREASURE_CHEST);

		// The iron sword gets leaked when created like this
		List<IItem> items = new()
		{
			CreateHealingPotion()
		};

		Equipment? equipment = GetRandomWeapon();

		if (equipment != null)
			items.Add(equipment);

		// items.Add(CreateIronSword());
		ItemContainer itemContainer = new(items, TreasureChestMesh);
		this.AddChild(itemContainer);

		itemContainer.GlobalTransform =
			new Transform3D(itemContainer.GlobalTransform.Basis, location);

		kdTreeController.AddInteractable(itemContainer);
	}

	Equipment CreateIronSword()
	{
		Equipment equipment = new(EquipmentType.HELD, "iron sword")
		{
			BaseDamage = 7
		};

		return equipment;
	}

	Equipment CreateRustedDagger()
	{
		Equipment equipment = new(EquipmentType.HELD, "rusty dagger")
		{
			BaseDamage = 3
		};

		return equipment;
	}

	Equipment CreateLightSaber()
	{
		Equipment equipment = new(EquipmentType.HELD, "lightsaber")
		{
			BaseDamage = 15
		};

		return equipment;
	}

	Consumable CreateHealingPotion()
	{
		return new Consumable(40, "Health potion");
	}
}
