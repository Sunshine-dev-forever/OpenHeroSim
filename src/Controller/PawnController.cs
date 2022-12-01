using Godot;
using System;
using Serilog;
using Pawn.Tasks;
using System.Collections.Generic;

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
	private PawnBrain pawnBrain;
	private SensesController sensesController;
	private SensesStruct sensesStruct = new SensesStruct();



	//I am hardcoding all of the file paths, might change that later
	//Here lies all of the pawn Godot nodes:
	private HealthBar3D healthBar;
	private VisualController visualController;
	private CollisionShape collisionShape;
	private RayCast downwardRayCast;
	private NavigationAgent navigationAgent;
	private RigidBody rigidBody;
	//end all the pawn Godot Nodes


	private MovementController movementController;
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
