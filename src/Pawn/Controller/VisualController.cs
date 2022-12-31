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
		Vector3 scabbardTransform = new Vector3();
		Vector3 heldItemRotation = new Vector3();
		Vector3 heldItemTransform = new Vector3();

		Node currentWeapon = null!;

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
				currentWeapon = ironSword;
				//clear the children of scabbard and node
				foreach(Node node in heldItem.GetChildren()) {
					node.QueueFree();
				}
				foreach(Node node in scabbard.GetChildren()) {
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
			if(currentWeapon.GetParent() == null) {
				//we must have switched weapons recently
				heldItem.AddChild(currentWeapon);
			} else if( !(currentWeapon.GetParent().Equals(heldItem)) ) {
				currentWeapon.GetParent().RemoveChild(currentWeapon);
				heldItem.AddChild(currentWeapon);
			}
		}

		private void UpdatePawnVisualsForNonCombat() {
			if(currentWeapon.GetParent() == null) {
				//we must have switched weapons recently
				scabbard.AddChild(currentWeapon);
			} else if( !(currentWeapon.GetParent().Equals(scabbard)) ) {
				currentWeapon.GetParent().RemoveChild(currentWeapon);
				scabbard.AddChild(currentWeapon);
			}
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