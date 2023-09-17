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

namespace Worlds.MainTest;

public class MainTestRunner : IRunner
{

    readonly KdTreeController kdTreeController;
    //MainTestRunner will make children out of nodeStorage
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
            //CreateTestProjectile();
            NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");

            Vector3 location = new(4, 0, 4);
            PawnControllerBuilder.CreateTrainingDummy(location, nodeStorage, kdTreeController, navigation);
        }
        else if (input.IsActionPressed("ui_right"))
        {
            CreatePawnInCenter();
        }
    }
    double TimeSinceLastPawnCreation = 4;
    //lazy, bad coding
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

        Vector3 location = new(0, 5, 0);
        return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
                            .AddGoal(new HealGoal())
                            .AddGoal(new DefendSelfGoal())
                            .AddGoal(new LootGoal())
                            .AddGoal(new WanderGoal())
                            .AddAbility(AbilityDefinitions.THROW_ABILITY)
                            .AddAbility(AbilityDefinitions.STAB_ABILITY)
                            .WearEquipment(GetRandomWeapon())
                            .WearEquipment(GetHelmet())
                            .AddItem(CreateThrowable())
                            .Location(location)
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
        //I add an offset so the spear target's the pawns chest and not the pawn's feet
        Vector3 offset = new(0, 1, 0);
        ITargeting target = new InteractableTargeting(lastPawnSpawned, offset);
        Projectile projectile = new(spear, target, 50, true);
        nodeStorage.AddChild(projectile);
        projectile.GlobalTransform = new Transform3D(projectile.GlobalTransform.Basis, new Vector3(0, 5, 0));
    }

    IPawnController CreatePawn()
    {
        NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");

        Vector3 location = GenerateRandomVector();

        return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
                            .AddGoal(new HealGoal())
                            .AddGoal(new DefendSelfGoal())
                            .AddGoal(new LootGoal())
                            .AddGoal(new WanderGoal())
                            .WearEquipment(GetRandomWeapon())
                            .AddAbility(AbilityDefinitions.STAB_ABILITY)
                            .Location(location)
                            .Finish();
    }

    IPawnController CreatePawnInCenter()
    {
        NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");

        Vector3 location = new(0, 5, 0);

        return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
                            .AddGoal(new HealGoal())
                            .AddGoal(new DefendSelfGoal())
                            .AddAbility(AbilityDefinitions.STAB_ABILITY)
                            .AddGoal(new LootGoal())
                            .AddGoal(new WanderGoal())
                            .WearEquipment(GetRandomWeapon())
                            .WearEquipment(GetHelmet())
                            .Location(location)
                            .Finish();
    }

    Equipment GetHelmet()
    {
        Equipment equipment = new(EquipmentType.HEAD, "box helm");
        equipment.Defense = 5;
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
        switch (rng)
        {
            case 0:
                return CreateSpearMelee();
            case 1:
                return CreateRustedDagger();
            case 2:
                return CreateIronSword();
            default:
                return CreateRustedDagger();
        }
    }

    IPawnController CreateHealingPotionTester()
    {
        NavigationRegion3D navigation = nodeStorage.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
        return PawnControllerBuilder.Start(nodeStorage, kdTreeController, navigation)
                            .AddGoal(new HealGoal())
                            .AddGoal(new WanderGoal())
                            .Location(new Vector3(0, 5, 0))
                            .WearEquipment(CreateLightSaber())
                            .AddItem(CreateHealingPotion()).AddItem(CreateHealingPotion())
                            .DealDamage(50)
                            .Finish();
    }

    void CreateItemContainer()
    {
        Node3D TreasureChestMesh = CustomResourceLoader.LoadMesh(ResourcePaths.TREASURE_CHEST);
        //The iron sword gets leaked when created like this
        List<IItem> items = new();
        items.Add(CreateHealingPotion());
        items.Add(CreateIronSword());
        ItemContainer itemContainer = new(items, TreasureChestMesh);
        nodeStorage.AddChild(itemContainer);
        itemContainer.GlobalTransform = new Transform3D(itemContainer.GlobalTransform.Basis, new Vector3(0, 1, 0));
        kdTreeController.AddInteractable(itemContainer);
    }

    Equipment CreateIronSword()
    {
        Equipment equipment = new(EquipmentType.HELD, "iron sword");
        equipment.Damage = 5;
        return equipment;
    }

    Equipment CreateRustedDagger()
    {
        Equipment equipment = new(EquipmentType.HELD, "rusted dagger");
        equipment.Damage = 3;
        return equipment;
    }

    Equipment CreateLightSaber()
    {
        Equipment equipment = new(EquipmentType.HELD, "light saber");
        equipment.Damage = 10;
        return equipment;
    }
    Equipment CreateSpearMelee()
    {
        Equipment equipment = new(EquipmentType.HELD, "melee djerd");
        equipment.Damage = 2;
        return equipment;
    }

    Consumable CreateHealingPotion()
    {
        return new Consumable(40, "Health Potion");
    }
}
