using Godot;
using System;
using Serilog;
using System.Diagnostics;

namespace Pawn.Controller {
	public class MovementController
	{
		private NavigationAgent navigationAgent;
		private RayCast downwardRayCast;
		private RigidBody rigidBody;
		//TODO: need better name. Point a a lower part of the animation controller? 
		private VisualController visualController;

		private Vector3 originalLocationOfTarget = Vector3.Zero;

		//the Navigation Server can take some time to start up
		private bool isNavigationServerReady;

		public MovementController(RigidBody _rigidBody, VisualController _visualController, NavigationAgent _navigationAgent, RayCast _downwardRayCast) {
			rigidBody = _rigidBody;
			visualController = _visualController;
			navigationAgent = _navigationAgent;
			downwardRayCast = _downwardRayCast;
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
			Vector3 currentLocation = rigidBody.GlobalTransform.origin;
			Vector3 locationDiff = nextLocation - currentLocation;
			//Look in the direction of travel
			float newYRotation = (float) (Math.Atan2(locationDiff.x , locationDiff.z));
			visualController.setPawnRotation(newYRotation);
			
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
			if(navigationAgent.GetFinalLocation().DistanceTo(rigidBody.GlobalTransform.origin)
				< targetDistance) {
				return true;
			} else {
				return false;
			}
		}

		public void SetNavigation(Navigation navigation){
			navigationAgent.SetNavigation(navigation);
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
}
