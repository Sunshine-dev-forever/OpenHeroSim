using System.Collections.Generic;
using Godot;
using Pawn.Tasks;
using Serilog;
using Pawn.Item;

//Handles all animations and particle effects.. etc
namespace Pawn.Controller
{
	public class VisualController : Spatial
	{
		AnimationPlayer animationPlayer = null!;
		Spatial riggedCharacterRootNode = null!;

		BoneAttachment heldItemAttachment = null!;
		BoneAttachment scabbardAttachment = null!;

		Vector3 scabbardRotation = new Vector3();
		Vector3 scabbardOrigin = new Vector3();
		Vector3 heldItemRotation = new Vector3();
		Vector3 heldItemOrigin = new Vector3();

		Spatial currentWeapon = null!;
		Spatial? currentHeldItem;

		public override void _Ready()
		{
			//TODO need a better way of extracting this information
			animationPlayer = this.GetNode<AnimationPlayer>("RiggedCharacter/AnimationPlayer");
			riggedCharacterRootNode = this.GetNode<Spatial>("RiggedCharacter");
			heldItemAttachment = this.GetNode<BoneAttachment>("RiggedCharacter/Character/Skeleton/BoneAttachment");
			scabbardAttachment = this.GetNode<BoneAttachment>("RiggedCharacter/Character/Skeleton/BoneAttachment2");
			//clear the children of scabbard and node
			//There should really only be 1 child in both cases
			if(heldItemAttachment.GetChildCount() != 1 || scabbardAttachment.GetChildCount() != 1) {
					Log.Information("Child count of BoneAttachments not 1, what do?");
			}
			foreach(Node node in heldItemAttachment.GetChildren()) {
				//I know these have to be spatials
				Spatial betterNode = (Spatial) node;
				heldItemRotation = betterNode.Rotation;
				heldItemOrigin = betterNode.Transform.origin;
				node.QueueFree();
			}
			foreach(Node node in scabbardAttachment.GetChildren()) {
				Spatial betterNode = (Spatial) node;
				scabbardRotation = betterNode.Rotation;
				scabbardOrigin = betterNode.Transform.origin;
				node.QueueFree();
			}
		}

		public void ProcessTask(ITask task) {
			
			if(task.Action.HeldItemMesh == null) {
				ClearHeldItem();
				return;
			} else if (task.Action.HeldItemMesh.Equals(currentHeldItem)) {
				return;
			}
			Spatial newMeshToHold = task.Action.HeldItemMesh;
			ClearHeldItem();
			if(newMeshToHold.GetParent() != null) {
				//For some reason the mesh is parented elsewhere, now it is in our hand
				newMeshToHold.GetParent().RemoveChild(newMeshToHold);
			}
			heldItemAttachment.AddChild(newMeshToHold);
			newMeshToHold.Rotation = heldItemRotation;
			newMeshToHold.Transform = new Transform(currentWeapon.Transform.basis, heldItemOrigin);
		}

		private void ClearHeldItem() {
			if(currentHeldItem == null) {
				return;
			}
			currentHeldItem.GetParent().RemoveChild(currentHeldItem);
			if(currentHeldItem.Equals(currentWeapon)) {
				//we need to put weapon in scabbard
				PutWeaponInScabbard();
			}
		}
		public void SetWeapon(Weapon weapon) {
			currentWeapon = weapon.Mesh;
			PutWeaponInScabbard();
		}

		public void PutWeaponInScabbard() {
			scabbardAttachment.AddChild(currentWeapon);
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