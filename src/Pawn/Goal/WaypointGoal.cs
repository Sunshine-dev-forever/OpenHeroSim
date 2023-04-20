using System.Reflection.Metadata;
using System;
using Serilog;
using System.Collections.Generic;
using Pawn.Tasks;
using Pawn.Action;
using Pawn.Controller;
using Pawn.Item;
using Pawn.Targeting;
using Pawn.Controller.Components;
using Godot;
namespace Pawn.Goal {
	public partial class WaypointGoal : IPawnGoal
	{
		private List<Node3D> waypoints;
		private int waypointIndex = 0;
		private readonly static int GOAL_DISTANCE = 3;

		public WaypointGoal(List<Node3D> _waypoints) {
			waypoints = _waypoints;
		}

		public ITask GetTask(PawnController pawnController, SensesStruct sensesStruct) {
			if(waypointIndex == waypoints.Count) {
				//we have finished all the waypoints yay!
				return new InvalidTask();
			}
			Node3D currentWaypoint = waypoints[waypointIndex];
			//we want to get as close as we can to the next waypoint
			//movement controller has its own distance check which ensure that the pawn can actually reach the position in question
			//What I am trying to say is the optimally this function would check the distance bettween the current pawn location,
			//and the closest point that the pawn could get to the target given the current nav mesh
			//also we are checking positional coordinates in 3D
			//NoNos all around
			if (pawnController.GlobalTransform.Origin.DistanceTo(currentWaypoint.GlobalTransform.Origin) < GOAL_DISTANCE) {
				waypointIndex++;
				return GetTask(pawnController, sensesStruct);
			}
			//TODO: I dont actually want to stop here, I should be able to handle null action tasks
			int waitTimeMilliseconds = 10;
			IAction action = ActionBuilder.Start(pawnController, () => {})
										.Animation(AnimationName.Idle)
										.AnimationPlayLength(waitTimeMilliseconds)
										.Finish();
			ITargeting targeting = new StaticPointTargeting(currentWaypoint.GlobalTransform.Origin);
			return new Task(targeting, action);
		}
	}
}
