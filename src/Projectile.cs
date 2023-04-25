

//not sure where to put this class
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Serilog;
using KdTree;
using KdTree.Math;
using System.Linq;
using Pawn;
using Pawn.Targeting;

//the projectile class is just a bit of eye candy and does not ahve the ability to do anything... yet
public partial class Projectile : Node3D
{
	private Node3D mesh;
	private ITargeting target;
	private float speed;
	private static float GOAL_DISTANCE = 2;
	public Projectile(Node3D _mesh, ITargeting _target, float _speed) {
		mesh = _mesh;
		target = _target;
		speed = _speed;
		this.AddChild(mesh);
		//rotating the x by -90 degrees should point the projectile forward
		mesh.RotationDegrees = new Vector3(-90, 0,0);
	}
	public override void _Process(double delta)
	{
		if(!target.IsValid) {
			//Queuefree should remove all children
			this.QueueFree();
		}
		
		Vector3 targetLocation = target.GetTargetPosition();
		this.LookAt(targetLocation, Vector3.Up);
		Vector3 locationDiff = targetLocation - this.GlobalTransform.Origin;
		if(locationDiff.Length() < GOAL_DISTANCE) {
			this.QueueFree();
		}
		Vector3 goalDirection = locationDiff.Normalized();
		Vector3 newLocation = this.GlobalTransform.Origin + (goalDirection * (float) (speed * delta));
		this.GlobalTransform = new Transform3D(this.GlobalTransform.Basis, newLocation);
	}

}
