

//not sure where to put this class
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Serilog;
using KdTree;
using KdTree.Math;
using System.Linq;
using Pawn.Controller;
using Pawn.Targeting;

//the projectile class is just a bit of eye candy and does not ahve the ability to do anything... yet
public class Projectile : Spatial
{
	private Spatial mesh;
	private ITargeting target;
	private float speed;
	private static float GOAL_DISTANCE = 2;
	public Projectile(Spatial _mesh, ITargeting _target, float _speed) {
		mesh = _mesh;
		target = _target;
		speed = _speed;
		this.AddChild(mesh);
		//rotating the x by -90 degrees should point the projectile forward
		mesh.RotationDegrees = new Vector3(-90, 0,0);
	}
	public override void _Process(float delta)
	{
		if(!target.IsValid) {
			//Queuefree should remove all children
			this.QueueFree();
		}
		
		Vector3 targetLocation = target.GetTargetLocation();
		this.LookAt(targetLocation, Vector3.Up);
		Vector3 locationDiff = targetLocation - this.GlobalTransform.origin;
		if(locationDiff.Length() < GOAL_DISTANCE) {
			this.QueueFree();
		}
		Vector3 goalDirection = locationDiff.Normalized();
		this.GlobalTransform = new Transform(this.GlobalTransform.basis, this.GlobalTransform.origin + (goalDirection * speed * delta));
	}

}
