using Godot;
using System;
using Serilog;
using System.Diagnostics;
public class MovementController : Spatial
{
	private NavigationAgent? navigationAgent;
	private RayCast downwardRayCast;
	private GeneralUtil generalUtil = new GeneralUtil();
	[Export] private NodePath rigidBodyPath = "";
	private RigidBody rigidBody;
	[Export] private NodePath bodyMeshInstancePath = "";
	private MeshInstance bodyMeshInstance;

	private Vector3 originalLocationOfTarget = Vector3.Zero;

	//the Navigation Server can take some time to start up
	private bool isNavigationServerReady;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		generalUtil.Assert(rigidBodyPath != null, "rigidBodyPath was uninitialized");
		rigidBody = GetNode<RigidBody>(rigidBodyPath);

		generalUtil.Assert(bodyMeshInstancePath != null, "bodyMeshInstancePath was uninitialized");
		bodyMeshInstance = GetNode<MeshInstance>(bodyMeshInstancePath);

		navigationAgent = this.GetNode<NavigationAgent>("NavigationAgent");
		downwardRayCast = this.GetNode<RayCast>("RayCast");
	}

	//ProcessMovement should be called in the _PhysicsProcess function
	public void ProcessMovement(Vector3 targetLocation, float speed)
	{
		if(!isNavigationServerReady){
			UpdateIsNavigationServerReady();
			return;
		}

		//TODO: dont update path for every minor change in position
		if(targetLocation != originalLocationOfTarget){
			navigationAgent.SetTargetLocation(targetLocation);
			originalLocationOfTarget = targetLocation;
		}

		Vector3 floorNormal;
		if(downwardRayCast.IsColliding()){
			floorNormal = downwardRayCast.GetCollisionNormal();
		} else {
			//if in air then let physics take over and just fall
			return;
		}
		Vector3 nextLocation = navigationAgent.GetNextLocation();
		Vector3 currentLocation = this.GlobalTransform.origin;
		Vector3 locationDiff = nextLocation - currentLocation;
		//Look in the direction of travel
		//To be honest, I am not sure why I need to add PI/2, but it makes things work
		//TODO: figure out why I need to add PI/2
		float newZRotation = (float) ((Math.PI/2) + Math.Atan2(locationDiff.z,locationDiff.x));
		bodyMeshInstance.Rotation = new Vector3(0,0, newZRotation);


		Vector3 velocity = (nextLocation - currentLocation).Slide(floorNormal).Normalized() * speed;
		rigidBody.LinearVelocity = velocity;
	}

	//Stops the pawn in place
	public void Stop() {
		//Cant stop if we are in the air
		if(downwardRayCast.IsColliding()){
			rigidBody.LinearVelocity = Vector3.Zero;
		} 
	}

	/*
	*right now i am going of the navigation agents final destination to determine
	*if the final location has been reached
	*NOTE: the final location will always be locationed on the navigation mesh
	*/
	public bool HasFinishedMovement(float targetDistance){
		if(navigationAgent.GetFinalLocation().DistanceTo(this.GlobalTransform.origin)
			< targetDistance) {
			return true;
		} else {
			return false;
		}
	}

	private void UpdateIsNavigationServerReady(){
		RID mapRid = NavigationServer.AgentGetMap(navigationAgent.GetRid());
		//an ID of 0 should always be an invalid ID
		//I use the ID of 0 to check if the Navigation server has started up
		//This method might not be fullproof, so this is a possible source of bugs
		if(mapRid.GetId() == 0) {
			isNavigationServerReady = false;
		} else {
			isNavigationServerReady = true;
		}
	}

	//returns the bearing to the point given 
	//assumes x is horizontal axis, y is vertical axis
	//gives bearing relative to the horizontal negative axis
	//such that quadrant 2 has the lowest bearings
	private double GetBearingTo(Vector2 point) {
		//Dont question it, it works :)
		return Math.PI - Math.Atan2(point.y, point.x);
	}

}
