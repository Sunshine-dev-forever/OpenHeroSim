using Godot;
using System;
using System.Collections.Generic;
using Pawn;
using Util;
using Item;
using Pawn.Goal;
using Pawn.Action.Ability;

namespace Worlds;

public class PawnGenerator
{
	Node nodeStorage;
	KdTreeController kdTreeController;
	NavigationRegion3D navigationRegion3D;

	public PawnGenerator(Node _nodeStorage, KdTreeController _kdTreeController, NavigationRegion3D _navigationRegion3D)
	{
		kdTreeController = _kdTreeController;
		nodeStorage = _nodeStorage;
		navigationRegion3D = _navigationRegion3D;
	}

	public IPawnController RandomPawn(IList<IPawnGoal> pawnGoals, Vector3 location, bool addRandomItems = false)
	{
		PawnControllerBuilder pawnControllerBuilder =
			PawnControllerBuilder.Start(
				nodeStorage,
				kdTreeController,
				navigationRegion3D)
			.Location(location)
			.SetGoals(pawnGoals)
			.Faction(IPawnInformation.NO_FACTION)
			.AddItem(StartingMoney());

		if (addRandomItems)
			AddRandomItems(pawnControllerBuilder);

		Random ran = new();

		int rng = ran.Next(0, 3);

		return rng switch
		{
			0 => CreateGoblin(pawnControllerBuilder),
			1 => CreateRogue(pawnControllerBuilder),
			2 => CreateWarrior(pawnControllerBuilder),
			_ => CreateWarrior(pawnControllerBuilder),
		};
	}

	private IItem StartingMoney()
	{
		return new StackItem(1, StackItem.MONEY);
	}

	PawnControllerBuilder AddRandomItems(PawnControllerBuilder pawnControllerBuilder)
	{
		Random ran = new();

		int rng = ran.Next(0, 3);

		return rng switch
		{
			0 => pawnControllerBuilder.AddItem(CreateIronSword()),
			1 => pawnControllerBuilder.AddItem(CreateRustedDagger()),
			2 => pawnControllerBuilder.AddItem(CreateHealingPotion()),
			_ => pawnControllerBuilder.AddItem(CreateHealingPotion()),
		};
	}

	IPawnController CreateGoblin(PawnControllerBuilder pawnControllerBuilder)
	{
		Random ran = new();

		pawnControllerBuilder.SetSpeed(14 + ran.Next(-3, 4));
		pawnControllerBuilder.SetMaxHealth(70 + ran.Next(-30, 40));
		pawnControllerBuilder.SetBaseDamage(25 + ran.Next(-10, 10));

		return pawnControllerBuilder
			.SetPawnRig(ResourcePaths.GOBLIN_MODEL)
			.AddAbility(AbilityDefinitions.STAB_ABILITY)
			.Finish();
	}

	IPawnController CreateRogue(PawnControllerBuilder pawnControllerBuilder)
	{
		Random ran = new();

		pawnControllerBuilder.SetSpeed(10 + ran.Next(-3, 4));
		pawnControllerBuilder.SetMaxHealth(90 + ran.Next(-30, 40));
		pawnControllerBuilder.SetBaseDamage(10 + ran.Next(-10, 10));

		return pawnControllerBuilder.SetPawnRig(ResourcePaths.ROGUE_MODEL)
			//.AddAbility(AbilityDefinitions.STAB_ABILITY)
			.AddAbility(AbilityDefinitions.THROW_ABILITY) // throw being shoot haha
			.AddItem(CreateInfiniteAmmo())
			.Finish();
	}

	IPawnController CreateWarrior(PawnControllerBuilder pawnControllerBuilder)
	{
		Random ran = new();

		pawnControllerBuilder.SetSpeed(9 + ran.Next(-3, 4));
		pawnControllerBuilder.SetMaxHealth(180 + ran.Next(-60, 70));
		pawnControllerBuilder.SetBaseDamage(12 + ran.Next(-10, 10));

		return pawnControllerBuilder
			.SetPawnRig(ResourcePaths.WARRIOR_MODEL)
			.AddAbility(AbilityDefinitions.STAB_ABILITY)
			.Finish();
	}

	Throwable CreateInfiniteAmmo()
	{
		Node3D arrow = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
		return new Throwable(arrow, 25, "arrow") { Count = 9999 };
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
