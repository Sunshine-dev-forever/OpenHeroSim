using Godot;
using GUI;
using GUI.DebugInspector.Display;
using Interactable;
using Item;
using Pawn.Action.Ability;
using Pawn.Components;
using Pawn.Tasks;
using System;
using Util;

namespace Pawn;

//HAVE TO CALL Setup() before this class will function!!!
public partial class PawnController : Node, IPawnController
{
    static readonly int TIME_TO_WAIT_AFTER_DEATH = 4;
    public Transform3D GlobalTransform
    {
        get => rigidBody.GlobalTransform;
        set => rigidBody.GlobalTransform = value;
    }
    public IPawnInformation PawnInformation { get; }
    public IPawnInventory PawnInventory { get; }

    public PawnBrain PawnBrain { get; }

    SensesStruct sensesStruct;
    ITask currentTask = new InvalidTask();

    //ALL OF THE BELOW VARIABLES ARE CREATED IN Setup() or _Ready()
    PawnSenses sensesController = null!;
    HealthBar3D healthBar = null!;
    public PawnTaskHandler PawnTaskHandler { get; set; } = null!;
    public PawnVisuals PawnVisuals { get; set; } = null!;

    CollisionShape3D collisionShape = null!;
    RayCast3D downwardRayCast = null!;
    NavigationAgent3D navigationAgent = null!;
    RigidBody3D rigidBody = null!;
    public PawnMovement PawnMovement { get; set; } = null!;

    KdTreeController KdTreeController = null!;
    //end variables which are created in Setup() or _Ready()

    //if death has been started, then this pawn is in the process of Dying
    public bool IsDying => startedDeath != DateTime.MaxValue;

    public IDisplay Display => ConstructDisplay();

    DateTime startedDeath = DateTime.MaxValue;
    ItemContainer? gravestone;

    //When created by instancing a scene, the default constructor is called.
    public PawnController()
    {
        sensesStruct = new SensesStruct();
        PawnBrain = new PawnBrain();
        PawnInventory = new PawnInventory();
        PawnInformation = new PawnInformation();
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //so paths are *ONLY* referenced here so I think I will just leave it hard coded
        healthBar = GetNode<HealthBar3D>("RigidBody3D/HealthBar");
        PawnVisuals = GetNode<PawnVisuals>("RigidBody3D/PawnVisuals");
        collisionShape = GetNode<CollisionShape3D>("RigidBody3D/CollisionShape3D");
        downwardRayCast = GetNode<RayCast3D>("RigidBody3D/DownwardRayCast");
        navigationAgent = GetNode<NavigationAgent3D>("RigidBody3D/NavigationAgent3D");
        rigidBody = GetNode<RigidBody3D>("RigidBody3D");

        PawnMovement = new PawnMovement(rigidBody,
                                                    PawnVisuals,
                                                    navigationAgent,
                                                    downwardRayCast);
        PawnTaskHandler = new PawnTaskHandler(PawnMovement, PawnVisuals, PawnInformation, PawnInventory);
    }

    //Setup HAS to be called for a pawn to work
    //For a pawn to function: the constructor is called,
    //then _Ready is called
    //Then Setup has to be called
    //Setup basically fills in for an actual constructor since I cannot seem to call a real constructor with args
    //    with the the PackedScene.Instance function
    public void Setup(KdTreeController kdTreeController)
    {
        KdTreeController = kdTreeController;
        sensesController = new PawnSenses(kdTreeController, this);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (IsDying)
        {
            HandleDying();
        }
        else
        {
            //only living pawns get to think
            sensesStruct = sensesController.UpdatePawnSenses(sensesStruct);
            currentTask = PawnBrain.updateCurrentTask(currentTask, sensesStruct, this);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsDying)
        {
            //Placed in physics process since Handle task may call some functions
            //in movement controller which **I Think** means this function has to be placed in 
            //_PhysicsProcess
            PawnTaskHandler.HandleTask(currentTask, this);
        }
    }

    void HandleDying()
    {
        if ((DateTime.Now - startedDeath).TotalSeconds > TIME_TO_WAIT_AFTER_DEATH)
        {
            //Corpse has gone cold for TIME_WAIT_AFTER_DEATH seconds, we finally pass
            if (gravestone != null && gravestone.IsInstanceValid())
            {
                gravestone.Visible = true;
            }

            FreeSelf();
        }
    }

    void StartDying()
    {
        startedDeath = DateTime.Now;
        PawnVisuals.SetAnimation(AnimationName.Die);
        CreateGravestone();
        //TODO: cannot figure out how to 'turn off' the rigid body
        //	such that the pawn does not collide with other pawns but still collides
        //	with the floor
    }

    //Gets the total damage that this pawn is able to produce.
    public double GetDamage()
    {
        return PawnInformation.BaseDamage + PawnInventory.GetTotalEquiptmentDamage();
    }

    //makes the pawn take damage
    //damage below 0 is ignored
    public void TakeDamage(double damage)
    {
        double taken_damage = damage - PawnInventory.GetTotalEquiptmentDefense();
        if (taken_damage < 0)
        {
            return;
        }

        PawnInformation.Health = PawnInformation.Health - taken_damage;
        if (PawnInformation.Health <= 0)
        {
            healthBar.SetHealthPercent(0);
            StartDying();
            return;
        }

        healthBar.SetHealthPercent(PawnInformation.Health / PawnInformation.MaxHealth);
    }

    //makes the pawn heal
    //healing below 0 is ignored
    public void TakeHealing(double amount)
    {
        if (amount < 0)
        {
            return;
        }

        PawnInformation.Health = PawnInformation.Health + amount;
        if (PawnInformation.Health > PawnInformation.MaxHealth)
        {
            PawnInformation.Health = PawnInformation.MaxHealth;
        }

        healthBar.SetHealthPercent(PawnInformation.Health / PawnInformation.MaxHealth);
    }

    void FreeSelf()
    {
        //free all memory
        //PawnInventory really should not have a reference to anything, since the gravestone should contain
        //   all of this pawns items, but whatever
        PawnInventory.QueueFree();
        this.QueueFree();
    }

    void CreateGravestone()
    {
        Node3D TreasureChestMesh = CustomResourceLoader.LoadMesh(ResourcePaths.GRAVESTONE);
        gravestone = new ItemContainer(PawnInventory.EmptyAllItems(), TreasureChestMesh);
        //Add newly created object to this objects current parent
        //This might be an issue somehow... but probably not now
        this.GetParent().AddChild(gravestone);
        gravestone.GlobalTransform = new Transform3D(gravestone.GlobalTransform.Basis, this.GlobalTransform.Origin);
        KdTreeController.AddInteractable(gravestone);
        //make the gravestone invisible so it looks like other pawns are looting the body
        gravestone.Visible = false;
    }

    public bool IsInstanceValid()
    {
        return IsInstanceValid(this);
    }

    public Node GetRootNode()
    {
        return this;
    }

    IDisplay ConstructDisplay()
    {

        Display root = new(PawnInformation.Name);
        root.AddDetail("Faction: " + PawnInformation.Faction);
        root.AddDetail("Base Attack: " + PawnInformation.BaseDamage);
        root.AddDetail("Max Health: " + PawnInformation.MaxHealth);
        root.AddDetail("Current Health: " + PawnInformation.Health);
        root.AddDetail("Speed: " + PawnInformation.Speed);

        Display childAbilities = new("Abilities");
        foreach (IAbility ability in PawnInformation.GetAllAbilites())
        {
            childAbilities.AddDetail("Ability: " + ability.Name);
        }

        root.AddChildDisplay(childAbilities);

        Display childEquipment = new("Equiptment");
        foreach (IItem item in PawnInventory.GetAllEquippedItems())
        {
            childEquipment.AddChildDisplay(item.Display);
        }

        root.AddChildDisplay(childEquipment);

        Display childBaggedItems = new("bagged Items");
        foreach (IItem item in PawnInventory.GetAllItemsInBag())
        {
            childBaggedItems.AddChildDisplay(item.Display);
        }

        childBaggedItems.AddDetail("number of bagged items: " + PawnInventory.GetAllItemsInBag().Count);
        root.AddChildDisplay(childBaggedItems);

        root.AddChildDisplay(currentTask.Display);

        return root;
    }
}
