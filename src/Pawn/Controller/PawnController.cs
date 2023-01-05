using System.Globalization;
using Godot;
using System;
using Serilog;
using Pawn.Tasks;
using System.Collections.Generic;
using Pawn.Item;

//HAVE TO CALL Setup() before this node will function!!!
namespace Pawn.Controller
{
	public class PawnController : Node, IInteractable
	{
		//TODO: This might be bad design, time will tell
		public Transform GlobalTransform
		{
			get { return rigidBody.GlobalTransform; }
			set { rigidBody.GlobalTransform = value; }
		}

		//TODO should all be in a pawnInfomration class
		public double health {get; private set;} = 100;
		private double maxHealth = 100;
		private double pawnBaseDamageAbility = 10;
		private string faction = "none";
		
		//TODO: have a non-null representation of weapon
		public Weapon Weapon {get; set;} = null!;
		public List<Consumable> ItemList = new List<Consumable>();
		public string pawnName = "Testy Mc Testerson";
		public ActionController ActionController {get;}
		public PawnBrainController PawnBrain {get;}
		private SensesStruct sensesStruct;

		//ALL OF THE BELOW VARIABLES ARE CREATED IN Setup() or _Ready
		private SensesController sensesController = null!;
		private HealthBar3D healthBar = null!;
		public VisualController VisualController {get; private set;} = null!;
		private CollisionShape collisionShape = null!;
		private RayCast downwardRayCast = null!;
		private NavigationAgent navigationAgent = null!;
		private RigidBody rigidBody = null!;
		public MovementController MovementController {get; private set;} = null!;


		private ITask currentTask = new InvalidTask();

		//TODO: implement something like the below:
		//private List<IPawnTrait> pawnTraits;

		//When created by instancing a scene, the default constructor is called.
		public PawnController() {
			ActionController = new ActionController();
			sensesStruct = new SensesStruct();
			PawnBrain = new PawnBrainController(ActionController);
		}
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			//so paths are *ONLY* reference here so I think I will just leave it hard coded
			healthBar = GetNode<HealthBar3D>("RigidBody/HealthBar");
			VisualController = GetNode<VisualController>("RigidBody/VisualController");
			collisionShape = GetNode<CollisionShape>("RigidBody/CollisionShape");
			downwardRayCast = GetNode<RayCast>("RigidBody/DownwardRayCast");
			navigationAgent = GetNode<NavigationAgent>("RigidBody/NavigationAgent");
			rigidBody = GetNode<RigidBody>("RigidBody");

			MovementController = new MovementController(rigidBody,
														VisualController,
														navigationAgent,
														downwardRayCast);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(float delta)
		{
			sensesStruct = sensesController.UpdatePawnSenses(sensesStruct);
			currentTask = PawnBrain.updateCurrentTask(currentTask, sensesStruct, this);
			VisualController.ProcessTask(currentTask);
		}

		public override void _PhysicsProcess(float delta)
		{
			ActionController.HandleTask(currentTask, MovementController, VisualController);
		}

		public void Setup(KdTreeController kdTreeController)
		{
			sensesController = new SensesController(kdTreeController, this);
		}

		//Gets the total damage that this pawn is able to produce.
		//TODO: needs to be replaced with actual pawn stats
		public double GetDamage() {
			if(Weapon != null) {
				return pawnBaseDamageAbility + Weapon.Damage;
			} else {
				return pawnBaseDamageAbility;
			}
		}

		//TODO: This should be handled by a pawnInventory class or something
		public void SetWeapon(Weapon _weapon) {
			Weapon = _weapon;
			VisualController.SetWeapon(Weapon);
		}


		public void TakeDamage(double damage)
		{
			health = health - damage;
			if (health <= 0)
			{
				Die();
				return;
			}
			if(health > maxHealth) {
				health = maxHealth;
			}
			healthBar.SetHealthPercent(health / maxHealth);
		}

		private void Die()
		{
			ItemList.ForEach( (item) => item.Mesh.QueueFree());
			this.QueueFree();
		}

		public void Adhoc()
		{
		}

		//only used by UI elements 
		//NEVER MUTATE THE TASK!!!
		public ITask GetTask()
		{
			return currentTask;
		}

		public bool IsInstanceValid() {
			return IsInstanceValid(this);
		}
	}
}
