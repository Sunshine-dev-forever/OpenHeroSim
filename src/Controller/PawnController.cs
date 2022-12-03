using Godot;
using System;
using Serilog;
using Pawn.Tasks;
using System.Collections.Generic;

//HAVE TO CALL Setup() before this node will function!!!
public class PawnController : Node
{
	//TODO: This might be bad design, time will tell
	public Transform GlobalTransform { 
		get { return rigidBody.GlobalTransform; }
		set { rigidBody.GlobalTransform = value; }
		}

	//TODO should all be in a pawnInfomration class
	[Export] private float health = 100;
	[Export] private float maxHealth = 100;
	//[Export] private readonly string PAWN_NAME = "Example Pawn";
	[Export] private string faction = "none";

	private GeneralUtil generalUtil = new GeneralUtil();
	
	private ActionController actionController = new ActionController();
	
	private SensesStruct sensesStruct = new SensesStruct();

	//ALL OF THE BELOW VARIABLES ARE CREATED IN Setup() or _Ready
	private PawnBrain pawnBrain = null!;
	private SensesController sensesController = null!;
	private HealthBar3D healthBar = null!;
	private VisualController visualController = null!;
	private CollisionShape collisionShape = null!;
	private RayCast downwardRayCast = null!;
	private NavigationAgent navigationAgent = null!;
	private RigidBody rigidBody = null!;
	private MovementController movementController = null!;


	private ITask currentTask = new InvalidTask();

	//TODO: These should be attributes of the task


	//TODO: implement something like the below:
	//private List<IPawnTrait> pawnTraits;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//so paths are *ONLY* reference here so I think I will just leave it hard coded
		healthBar = GetNode<HealthBar3D>("RigidBody/HealthBar");
		visualController = GetNode<VisualController>("RigidBody/VisualController");
		collisionShape = GetNode<CollisionShape>("RigidBody/CollisionShape");
		downwardRayCast = GetNode<RayCast>("RigidBody/DownwardRayCast");
		navigationAgent = GetNode<NavigationAgent>("RigidBody/NavigationAgent");
		rigidBody = GetNode<RigidBody>("RigidBody");

		movementController = new MovementController(rigidBody, 
													visualController.GetRiggedCharacterRootNode(),
													navigationAgent, 
													downwardRayCast);
		pawnBrain = new PawnBrain(actionController);
	}

	 // Called every frame. 'delta' is the elapsed time since the previous frame.
	 public override void _Process(float delta)
	 {
		sensesStruct = sensesController.UpdatePawnSenses(sensesStruct);
		currentTask = pawnBrain.updateCurrentTask(currentTask, sensesStruct, this);
	 }

	public override void _PhysicsProcess(float delta)
	{
		actionController.HandleTask(currentTask, movementController, visualController);
	}

	public void Setup(KdTreeController kdTreeController) {
		sensesController = new SensesController(kdTreeController, this);
	}


	 public void TakeDamage(float damage) {
		health = health - damage;
		healthBar.SetHealthPercent(health/maxHealth);
		if(health < 0) {
			Die();
		}
	 }

	private void Die() {
		this.QueueFree();
	}

	public void Adhoc() {
		AnimationPlayer animationPlayer = visualController.GetAnimationPlayer();
		animationPlayer.GetAnimation("Walking").Loop = true;
		animationPlayer.Play("Walking");
	}
}
