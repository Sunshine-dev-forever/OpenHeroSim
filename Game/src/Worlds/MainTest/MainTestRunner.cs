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
using GUI.DebugInspector;

namespace Worlds.MainTest;

public partial class MainTestRunner : Node
{
    KdTreeController kdTreeController = null!;
    PawnGenerator pawnGenerator = null!;
    double TimeSinceLastPawnCreation = 4;
    // lazy, bad coding
    IPawnController lastPawnSpawned = null!;

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
        if (input.IsActionPressed("ui_left"))
        {
            string navPath = "/root/Node3D/NavigationRegion3D";

            // CreateTestProjectile();
            NavigationRegion3D navigation =
                this.GetNode<NavigationRegion3D>(navPath);

            Vector3 location = new(4, 0, 4);

            PawnControllerBuilder.CreateTrainingDummy(
                location,
                this,
                kdTreeController,
                navigation);
        }
        else if (input.IsActionPressed("ui_right"))
        {
            CreatePawnInCenter();
        }
    }

    public override void _Process(double delta)
    {
        TimeSinceLastPawnCreation += delta;

        if (TimeSinceLastPawnCreation > 5)
        {
            TimeSinceLastPawnCreation = 0;
            lastPawnSpawned = CreatePawn();
        }
        kdTreeController.Process(delta);
    }

    public IPawnController CreateThrowableTester()
    {
        NavigationRegion3D navigation = this.GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");

        return PawnControllerBuilder.Start(this, kdTreeController, navigation)
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
        this.AddChild(projectile);

        projectile.GlobalTransform =
            new Transform3D(
                basis: projectile.GlobalTransform.Basis,
                origin: new Vector3(0, 5, 0));
    }

    IPawnController CreatePawn()
    {
        string navPath = "/root/Node3D/NavigationRegion3D";

        NavigationRegion3D navigation =
            this.GetNode<NavigationRegion3D>(navPath);

        List<IPawnGoal> goals = new List<IPawnGoal> {
                new HealGoal(),
                new DefendSelfGoal(),
                new LootGoal(),
                new WanderGoal()
            };

        return pawnGenerator.RandomPawn(goals, GenerateRandomVector(), true);
    }

    IPawnController CreatePawnInCenter()
    {
        string navPath = "/root/Node3D/NavigationRegion3D";

        NavigationRegion3D navigation =
            this.GetNode<NavigationRegion3D>(navPath);

        return PawnControllerBuilder.Start(this, kdTreeController, navigation)
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
        this.AddChild(itemContainer);
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
