using System.Collections.Generic;
using Godot;
using Pawn.Tasks;
using Serilog;

//Handles all animations and particle effects.. etc
namespace Pawn.Controller
{
	public class VisualController : Spatial
	{
		AnimationPlayer animationPlayer = null!;
		Spatial riggedCharacterRootNode = null!;

		BoneAttachment heldItem = null!;
		BoneAttachment scabbard = null!;

		Vector3 scabbardRotation = new Vector3();
		Vector3 scabbardOrigin = new Vector3();
		Vector3 heldItemRotation = new Vector3();
		Vector3 heldItemOrigin = new Vector3();

		Spatial currentWeapon = null!;

		public override void _Ready()
		{
			//TODO need a better way of extracting this information
			animationPlayer = this.GetNode<AnimationPlayer>("RiggedCharacter/AnimationPlayer");
			riggedCharacterRootNode = this.GetNode<Spatial>("RiggedCharacter");
			heldItem = this.GetNode<BoneAttachment>("RiggedCharacter/Character/Skeleton/BoneAttachment");
			scabbard = this.GetNode<BoneAttachment>("RiggedCharacter/Character/Skeleton/BoneAttachment2");
		}

		bool firstTime = true;

		public void ProcessTask(ITask task) {
			if(firstTime) {
				firstTime = false;
				Node ironSword = GD.Load<PackedScene>("res://scenes/weapons/iron_sword.tscn").Instance();
				currentWeapon = (Spatial) ironSword;
				if(heldItem.GetChildCount() != 1 || scabbard.GetChildCount() != 1) {
					Log.Information("Child count of BoneAttachments not 1, what do?");
				}
				//clear the children of scabbard and node
				foreach(Node node in heldItem.GetChildren()) {
					//I know these have to be spatials
					Spatial betterNode = (Spatial) node;
					heldItemRotation = betterNode.Rotation;
					heldItemOrigin = betterNode.Transform.origin;
					node.QueueFree();
				}
				foreach(Node node in scabbard.GetChildren()) {
					Spatial betterNode = (Spatial) node;
					scabbardRotation = betterNode.Rotation;
					scabbardOrigin = betterNode.Transform.origin;
					node.QueueFree();
				}
				//heldItem.AddChild(ironSword);
				//I want to avoid duplicating things here
				//scabbard.AddChild(ironSword.Duplicate());
			}

			if(task.isCombat) {
				UpdatePawnVisualsForCombat();
			} else {
				UpdatePawnVisualsForNonCombat();
			}
		}

		private void UpdatePawnVisualsForCombat() {
			if(heldItem.Equals(currentWeapon.GetParent())) {
				//nothing to do here
				return;
			}

			if(currentWeapon.GetParent() != null) {
				currentWeapon.GetParent().RemoveChild(currentWeapon);
			}
			heldItem.AddChild(currentWeapon);
			currentWeapon.Rotation = heldItemRotation;
			currentWeapon.Transform = new Transform(currentWeapon.Transform.basis, heldItemOrigin);
		}

		private void UpdatePawnVisualsForNonCombat() {
			if(scabbard.Equals(currentWeapon.GetParent())) {
				//nothing to do here
				return;
			}

			if(currentWeapon.GetParent() != null) {
				currentWeapon.GetParent().RemoveChild(currentWeapon);
			}
			scabbard.AddChild(currentWeapon);
			currentWeapon.Rotation = scabbardRotation;
			currentWeapon.Transform = new Transform(currentWeapon.Transform.basis, scabbardOrigin);
		}

		public float getAnimationLengthMilliseconds(AnimationName animationName) {
			//TODO: find some constants library that defines this
			int MILLISECONDS_IN_SECOND = 1000;
			return animationPlayer.GetAnimation(animationName.ToString()).Length * MILLISECONDS_IN_SECOND;
		}

		public void SetAnimation(AnimationName animationName, bool looping = false) {
			animationPlayer.GetAnimation(animationName.ToString()).Loop = looping;
			animationPlayer.Play(animationName.ToString());
		}

		public void setPawnRotation(float yRotation) {
			//Only the y rotaiton should ever be changed
			riggedCharacterRootNode.Rotation = new Vector3(0,yRotation,0);
		}
	}

	public enum AnimationName {
		Death,
		Drink,
		Idle,
		Interact,
		Walking,
		Stab
	}
}