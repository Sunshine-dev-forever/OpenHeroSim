using Godot;
using System;
using System.Collections.Generic;
using Pawn;
using Util;
using Item;
using Pawn.Goal;
using Pawn.Action.Ability;

namespace Worlds;

public class PawnGenerator {
    readonly Node nodeStorage;
    readonly KdTreeController kdTreeController;
    readonly NavigationRegion3D navigationRegion3D;
    readonly Random random = new();

    public PawnGenerator(Node _nodeStorage, KdTreeController _kdTreeController, NavigationRegion3D _navigationRegion3D) {
        kdTreeController = _kdTreeController;
        nodeStorage = _nodeStorage;
        navigationRegion3D = _navigationRegion3D;
    }

    public IPawnController RandomPawn(IList<IPawnGoal> pawnGoals, Vector3 location, bool addRandomItems = false, List<PawnType>? pawnTypes = null) {

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

        return GetPawnTypeToSpawn(pawnTypes) switch {
            PawnType.GOBLIN => CreateGoblin(pawnControllerBuilder),
            PawnType.ROGUE => CreateRogue(pawnControllerBuilder),
            PawnType.WARRIOR => CreateWarrior(pawnControllerBuilder),
            _ => CreateWarrior(pawnControllerBuilder),
        };
    }

    private PawnType GetPawnTypeToSpawn(List<PawnType>? pawnTypes) {
        if (pawnTypes is null) {
            PawnType[] allPawnTypes = Enum.GetValues<PawnType>();
            return allPawnTypes[random.Next(allPawnTypes.Length)];
        }
        else {
            return pawnTypes[random.Next(pawnTypes.Count)];
        }
    }

    private IItem StartingMoney() => new StackItem(10, StackItem.MONEY);

    PawnControllerBuilder AddRandomItems(PawnControllerBuilder pawnControllerBuilder) {
        Random ran = new();

        int rng = ran.Next(0, 3);

        return rng switch {
            0 => pawnControllerBuilder.AddItem(CreateIronSword()),
            1 => pawnControllerBuilder.AddItem(CreateRustedDagger()),
            2 => pawnControllerBuilder.AddItem(CreateHealingPotion()),
            _ => pawnControllerBuilder.AddItem(CreateHealingPotion()),
        };
    }

    IPawnController CreateGoblin(PawnControllerBuilder pawnControllerBuilder) {
        Random ran = new();

        pawnControllerBuilder.SetSpeed(14 + ran.Next(-3, 4));
        pawnControllerBuilder.SetMaxHealth(70 + ran.Next(-30, 40));
        pawnControllerBuilder.SetBaseDamage(25 + ran.Next(-10, 10));

        return pawnControllerBuilder
            .SetPawnRig(ResourcePaths.GOBLIN_MODEL)
            .AddAbility(AbilityDefinitions.STAB_ABILITY)
            .Finish();
    }

    IPawnController CreateRogue(PawnControllerBuilder pawnControllerBuilder) {
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

    IPawnController CreateWarrior(PawnControllerBuilder pawnControllerBuilder) {
        Random ran = new();

        pawnControllerBuilder.SetSpeed(9 + ran.Next(-3, 4));
        pawnControllerBuilder.SetMaxHealth(180 + ran.Next(-60, 70));
        pawnControllerBuilder.SetBaseDamage(12 + ran.Next(-10, 10));

        return pawnControllerBuilder
            .SetPawnRig(ResourcePaths.WARRIOR_MODEL)
            .AddAbility(AbilityDefinitions.STAB_ABILITY)
            .Finish();
    }

    Throwable CreateInfiniteAmmo() {
        Node3D arrow = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
        return new Throwable(arrow, 25, "arrow") { Count = 9999 };
    }

    Equipment CreateIronSword() {
        Equipment equipment = new(EquipmentType.HELD, "iron sword") {
            BaseDamage = 7
        };

        return equipment;
    }

    Equipment CreateRustedDagger() {
        Equipment equipment = new(EquipmentType.HELD, "rusty dagger") {
            BaseDamage = 3
        };

        return equipment;
    }

    Equipment CreateLightSaber() {
        Equipment equipment = new(EquipmentType.HELD, "lightsaber") {
            BaseDamage = 15
        };

        return equipment;
    }

    Consumable CreateHealingPotion() => new(40, "Health potion");

}
