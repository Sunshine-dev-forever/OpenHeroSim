using Godot;
using System;
using Pawn.Tasks;
using Util;
using UI;
using Pawn.Components;
using Interactable;


namespace Pawn
{
	//HAVE TO CALL Setup() before this class will function!!!
	//TODO: this should have an interface!!!!
	public partial class PawnController : Node, IInteractable
	{
		private static int TIME_TO_WAIT_AFTER_DEATH = 2;
		public Transform3D GlobalTransform
		{
			get { return rigidBody.GlobalTransform; }
			set { rigidBody.GlobalTransform = value; }
		}
		public PawnInformation PawnInformation {get;}
		public PawnInventory PawnInventory {get;}

		public PawnBrain PawnBrain {get;}
		private SensesStruct sensesStruct;
		private ITask currentTask = new InvalidTask();

		//ALL OF THE BELOW VARIABLES ARE CREATED IN Setup() or _Ready()
		private PawnSenses sensesController = null!;
		private HealthBar3D healthBar = null!;
		public PawnTaskHandler PawnTaskHandler {get; private set;} = null!;
		public PawnVisuals PawnVisuals {get; private set;} = null!;
		private CollisionShape3D collisionShape = null!;
		private RayCast3D downwardRayCast = null!;
		private NavigationAgent3D navigationAgent = null!;
		private RigidBody3D rigidBody = null!;
		public PawnMovement PawnMovement {get; private set;} = null!;
		private KdTreeController KdTreeController = null!;
		//end variables which are created in Setup() or _Ready()

		//if death has been started, then this pawn is in the process of Dying
		public bool IsDying {get { return startedDeath != DateTime.MaxValue;}}
		private DateTime startedDeath = DateTime.MaxValue;

		//When created by instancing a scene, the default constructor is called.
		public PawnController() {
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
		//for the PackedScene.Instance function
		public void Setup(KdTreeController kdTreeController)
		{
			KdTreeController = kdTreeController;
			sensesController = new PawnSenses(kdTreeController, this);
			PawnVisuals.ForceVisualUpdate(PawnInventory);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if(IsDying) {
				HandleDying();
			} else {
				//only living pawns get to think
				sensesStruct = sensesController.UpdatePawnSenses(sensesStruct);
				currentTask = PawnBrain.updateCurrentTask(currentTask, sensesStruct, this);
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			if(!IsDying) {
				//Placed in physics process since Handle task may call some functions
				//in movement controller which **I Think** means this function has to be placed in 
				//_PhysicsProcess
				PawnTaskHandler.HandleTask(currentTask, this);
			}
		}

		public void HandleDying() {
			if( (DateTime.Now - startedDeath).TotalSeconds > TIME_TO_WAIT_AFTER_DEATH) {
				//Corpse has gone cold for 5 seconds, we finally pass
				Die();
			}
		}

		public void StartDying() {
			startedDeath = DateTime.Now;
			PawnVisuals.SetAnimation(AnimationName.Death);
			//TODO: cannot figure out how to 'turn off' the rigid body
			//	such that the pawn does not collide with other pawns but still collides
			//	with the floor
		}

		//Gets the total damage that this pawn is able to produce.
		public double GetDamage() {
			return PawnInformation.BaseDamage + PawnInventory.GetTotalEquiptmentDamage();
		}

		//makes the pawn take damage
		//damage below 0 is ignored
		public void TakeDamage(double damage)
		{
			double taken_damage = damage - PawnInventory.GetTotalEquiptmentDefense();
			if(taken_damage < 0) {
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
		public void TakeHealing(double amount) {
			if(amount < 0) {
				return;
			}
			PawnInformation.Health = PawnInformation.Health + amount;
			if(PawnInformation.Health > PawnInformation.MaxHealth) {
				PawnInformation.Health = PawnInformation.MaxHealth;
			}
			healthBar.SetHealthPercent(PawnInformation.Health / PawnInformation.MaxHealth);
		}

		private void Die()
		{
			CreateGraveStone();
			//free all memory
			//PawnInventory really should not have a reference to anything, since the gravestone should contain
			//all of this pawns items, but whatever
			PawnInventory.QueueFree();
			this.QueueFree();
		}

		private void CreateGraveStone() {
			Node3D TreasureChestMesh = CustomResourceLoader.LoadMesh(ResourcePaths.GRAVESTONE);
			ItemContainer itemContainer = new ItemContainer(PawnInventory.EmptyAllItems(), TreasureChestMesh);
			//Add newly created object to this objects current parent
			//This might be an issue somehow... but probably now
			this.GetParent().AddChild(itemContainer);
			itemContainer.GlobalTransform = new Transform3D(itemContainer.GlobalTransform.Basis, this.GlobalTransform.Origin);
			KdTreeController.AddInteractable(itemContainer);
		}

		public bool IsInstanceValid() {
			return IsInstanceValid(this);
		}
	}
}
