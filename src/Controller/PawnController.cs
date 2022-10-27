using Godot;
using System;
using Serilog;
using Pawn.Tasks;
using System.Collections.Generic;

public class PawnController : RigidBody
{
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

	//ill need to recreate healthBar in C# unless I want to deal with cancer when
	//calling the healthbar funcitons
	[Export] private NodePath healthBarPath = "";
	private HealthBar3D healthBar;
	[Export] private NodePath movementControllerPath = "";
	private MovementController movementController;

	private PawnState pawnState = PawnState.REST;
	private TaskState taskState = TaskState.COMPLETED;
	private ITask currentTask = new InvalidTask();
	private PawnController currentTarget;

	private int visionRange = 10;

	//TODO: unhardcode pawnstate
	enum PawnState { COMBAT, REST, ADVENTURE};
	enum TaskState { MOVING_TO, STARTING_ACTION, USING_ACTION, COMPLETED}

	//TODO: implement something like the below:
	//private List<IPawnTrait> pawnTraits;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		generalUtil.Assert(healthBarPath != null, "healthBarPath was not initialized");
		healthBar = GetNode<HealthBar3D>("HealthBar");

		generalUtil.Assert(movementControllerPath != null, "movementControllerPath was not initialized");
		movementController = GetNode<MovementController>(movementControllerPath);
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
}
