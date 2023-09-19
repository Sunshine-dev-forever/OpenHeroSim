using Godot;
using Interactable;
using Item;
using Pawn;
using Pawn.Action.Ability;
using Pawn.Goal;
using Pawn.Targeting;
using System;
using System.Collections.Generic;
using Util;
using System.Linq;

namespace Worlds.MainTest;

public class MainTestRunner : IRunner
{
    readonly KdTreeController kdTreeController;

    // MainTestRunner will make children out of nodeStorage
    readonly Node nodeStorage;

    public MainTestRunner(KdTreeController _kdTreeController, Node _nodeStorage)
    {
        kdTreeController = _kdTreeController;
        nodeStorage = _nodeStorage;
    }

    public void Input(InputEvent input)
    {
        if (input.IsActionPressed("ui_left"))
        {
            string navPath = "/root/Node3D/NavigationRegion3D";

            // CreateTestProjectile();
            NavigationRegion3D navigation = 
                nodeStorage.GetNode<NavigationRegion3D>(navPath);

            Vector3 location = new(4, 0, 4);

            PawnControllerBuilder.CreateTrainingDummy(
                location, 
                nodeStorage, 
                kdTreeController, 
                navigation);
        }
        else if (input.IsActionPressed("ui_right"))
        {
            CreatePawnInCenter();
        }
    }

    double TimeSinceLastPawnCreation = 4;

    // lazy, bad coding
    IPawnController lastPawnSpawned = null!;

    public void Process(double delta)
    {
        TimeSinceLastPawnCreation += delta;

        if (TimeSinceLastPawnCreation > 5)
        {
            TimeSinceLastPawnCreation = 0;
            lastPawnSpawned = CreatePawn();
        }
    }

    public IPawnController CreateThrowableTester()
    {
        NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");

        return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
            .SetGoals(new List<IPawnGoal> {
                new HealGoal(),
                new DefendSelfGoal(),
                new LootGoal(),
                new WanderGoal()
            })
            .AddAbility(AbilityDefinitions.THROW_ABILITY)
            .AddAbility(AbilityDefinitions.STAB_ABILITY)
            .WearEquipment(GetRandomWeapon())
            .WearEquipment(GetHelmet())
            .AddItem(CreateThrowable())
            .Location(new Vector3(0, 5, 0))
            .Finish();
    }

    public Throwable CreateThrowable()
    {
        Node3D spear = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
        return new Throwable(spear, 60, "throwing djerd");
    }

    void CreateTestProjectile()
    {
        Node3D spear = CustomResourceLoader.LoadMesh(ResourcePaths.DJERID);
        
        // I add an offset so the spear target's the pawns chest and not the pawn's feet
        Vector3 offset = new(0, 1, 0);
        
        ITargeting target = new InteractableTargeting(lastPawnSpawned, offset);
        
        Projectile projectile = new(spear, target, 50, true);
        nodeStorage.AddChild(projectile);
        
        projectile.GlobalTransform = 
            new Transform3D(
                basis: projectile.GlobalTransform.Basis, 
                origin: new Vector3(0, 5, 0));
    }

    IPawnController CreatePawn()
    {
        string navPath = "/root/Node3D/NavigationRegion3D";

        NavigationRegion3D navigation = 
            nodeStorage.GetNode<NavigationRegion3D>(navPath);

        return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
            .SetGoals(new List<IPawnGoal> {
                new HealGoal(),
                new DefendSelfGoal(),
                new LootGoal(),
                new WanderGoal()
            })
            .WearEquipment(GetRandomWeapon())
            .AddAbility(AbilityDefinitions.STAB_ABILITY)
            .Location(GenerateRandomVector())
            .Finish();
    }

    IPawnController CreatePawnInCenter()
    {
        string navPath = "/root/Node3D/NavigationRegion3D";

        NavigationRegion3D navigation = 
            nodeStorage.GetNode<NavigationRegion3D>(navPath);

        return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
            .SetGoals(new List<IPawnGoal> {
                new HealGoal(),
                new DefendSelfGoal(),
                new LootGoal(),
                new WanderGoal()
            })
            .AddAbility(AbilityDefinitions.STAB_ABILITY)
            .WearEquipment(GetRandomWeapon())
            .WearEquipment(GetHelmet())
            .Location(new Vector3(0, 5, 0))
            .Finish();
    }

    Equipment GetHelmet()
    {
        Equipment equipment = new(EquipmentType.HEAD, "box helm")
        {
            Defense = 5
        };

        return equipment;
    }

    Vector3 GenerateRandomVector()
    {
        Random rand = new();

        int rng = rand.Next(0, 3);

        if (rng == 0)
        {
            return new Vector3(23, 5, 23);
        }
        else if (rng == 1)
        {
            return new Vector3(-23, 5, 23);
        }
        else if (rng == 2)
        {
            return new Vector3(-23, 5, -23);
        }
        else if (rng == 3)
        {
            return new Vector3(23, 5, -23);
        }

        return new Vector3();
    }

    Equipment GetRandomWeapon()
    {
        Random rand = new();

        int rng = rand.Next(0, 3);

        return rng switch
        {
            0 => CreateSpearMelee(),
            1 => CreateRustedDagger(),
            2 => CreateIronSword(),
            _ => CreateRustedDagger(),
        };
    }

    void CreateItemContainer()
    {
        Node3D TreasureChestMesh = 
            CustomResourceLoader.LoadMesh(ResourcePaths.TREASURE_CHEST);

        // The iron sword gets leaked when created like this
        List<IItem> items = new()
        {
            CreateHealingPotion(),
            CreateIronSword()
        };

        ItemContainer itemContainer = new(items, TreasureChestMesh);
        nodeStorage.AddChild(itemContainer);
        itemContainer.GlobalTransform = new Transform3D(itemContainer.GlobalTransform.Basis, new Vector3(0, 1, 0));
        kdTreeController.AddInteractable(itemContainer);
    }

    Equipment CreateIronSword()
    {
        Equipment equipment = new(EquipmentType.HELD, "iron sword")
        {
            Damage = 5
        };

        return equipment;
    }

    Equipment CreateRustedDagger()
    {
        Equipment equipment = new(EquipmentType.HELD, "rusted dagger")
        {
            Damage = 3
        };

        return equipment;
    }

    Equipment CreateLightSaber()
    {
        Equipment equipment = new(EquipmentType.HELD, "light saber")
        {
            Damage = 10
        };

        return equipment;
    }

    Equipment CreateSpearMelee()
    {
        Equipment equipment = new(EquipmentType.HELD, "melee djerd")
        {
            Damage = 2
        };

        return equipment;
    }

    Consumable CreateHealingPotion()
    {
        return new Consumable(40, "Health Potion");
    }
}
