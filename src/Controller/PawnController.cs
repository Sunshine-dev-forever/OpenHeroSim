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
	private PawnBrain pawnBrain = new PawnBrain();
	private ActionController actionController = new ActionController();
	private SensesController sensesController;
	private SensesStruct sensesStruct = new SensesStruct();
	private PawnCombatBrain pawnCombatBrain = new PawnCombatBrain();



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
	private PawnState pawnState = PawnState.REST;
	private TaskState taskState = TaskState.COMPLETED;
	private ITask currentTask = new InvalidTask();

	//TODO this should not exist, replace with proper vision
	private PawnController currentTarget;

	//TODO: should be in a pawnInformation or pawn statistics class
	private int visionRange = 10;

	//TODO: unhardcode pawnstate
	enum PawnState { COMBAT, REST, ADVENTURE};
	enum TaskState { MOVING_TO, STARTING_ACTION, USING_ACTION, COMPLETED}

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
	}

	 // Called every frame. 'delta' is the elapsed time since the previous frame.
	 public override void _Process(float delta)
	 {
		sensesStruct = sensesController.UpdatePawnSenses(sensesStruct);
		if(sensesStruct.nearbyPawns.Count > 0) {
			HandleOtherPawnInVision(sensesStruct.nearbyPawns[0]);
		}


		if(taskState == TaskState.STARTING_ACTION){
			actionController.executeActionFromTask(currentTask);
			taskState = TaskState.USING_ACTION;
		}
		if(taskState == TaskState.USING_ACTION) {
			if(actionController.isActionCompleted()) {
				taskState = TaskState.COMPLETED;
			}
		}

	 }

	public override void _PhysicsProcess(float delta)
	{
		if(taskState == TaskState.COMPLETED) {
			if(pawnState == PawnState.COMBAT){
				if(!IsInstanceValid(currentTarget)) {
					//our opponent is Dead!
					pawnState = PawnState.ADVENTURE;
					currentTask = pawnBrain.GetNextTask(this);
				} else {
					currentTask = pawnCombatBrain.GetNextTask(this, actionController, currentTarget);//TODO
				}
			} else {
				currentTask = pawnBrain.GetNextTask(this);
			}
			taskState = TaskState.MOVING_TO;
		}
		if(taskState == TaskState.MOVING_TO){
			int speed = 10;
			float targetDistance = currentTask.targetDistance;
			//Our task is now invalid, need to create a new one
			if(!currentTask.isValid) {
				taskState = TaskState.COMPLETED;
				return;
			}
			movementController.ProcessMovement(currentTask.GetTargetLocation(), speed);

			if(movementController.HasFinishedMovement(targetDistance)) {
				movementController.Stop();
				taskState = TaskState.STARTING_ACTION;
			}
		}
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

	private void HandleOtherPawnInVision(PawnController otherPawnController) {
		if(pawnState == PawnState.COMBAT){
			//We are already in combat
		} else {
			if(otherPawnController.faction == "none") {
				pawnState = PawnState.COMBAT;
				currentTarget = otherPawnController;
				if(taskState == TaskState.USING_ACTION || taskState == TaskState.STARTING_ACTION) {
					//TODO: I need a way to interrupt actions
				} else {
					taskState = TaskState.COMPLETED;
				}
			}
		}
	}

	public void Adhoc() {
		AnimationPlayer animationPlayer = visualController.GetAnimationPlayer();
		animationPlayer.GetAnimation("Walking").Loop = true;
		animationPlayer.Play("Walking");
	}
}
