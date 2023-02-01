using System.Collections.Generic;
using Godot;
using Pawn.Tasks;
using Serilog;
using Pawn.Item;
using Util;

//Handles all animations and particle effects.. etc
namespace Pawn.Controller
{
	public class VisualController : Spatial
	{
		AnimationPlayer animationPlayer = null!;
		Spatial riggedCharacterRootNode = null!;

		//TODO: All of the below should be in thier own class
		BoneAttachment heldItemAttachment = null!;
		BoneAttachment scabbardAttachment = null!;
		Vector3 scabbardRotation = new Vector3();
		Vector3 scabbardOrigin = new Vector3();
		Vector3 heldItemRotation = new Vector3();
		Vector3 heldItemOrigin = new Vector3();

		//Taking a guess for where to put the helmet
		Vector3 helmetOrigin = new Vector3(0,4,0);
		Vector3 helmetRotation = new Vector3();
		Spatial helmetParent = null!;
		//end scary instance variables

		IItem? currentHeldItem;
		public override void _Ready(){}

		//I really need a pawn rig loader
		public void SetPawnRig(string filename) {
			Spatial pawnMesh = ResourceLoader.Load<PackedScene>(filename).Instance<Spatial>();
			//clear all old nodes
			foreach (Node node in this.GetChildren()) {
				node.QueueFree();
			}
			this.AddChild(pawnMesh);
			//TODO need a better way of extracting this information
			animationPlayer = pawnMesh.GetNode<AnimationPlayer>("AnimationPlayer");
			riggedCharacterRootNode = pawnMesh;
			heldItemAttachment = pawnMesh.GetNode<BoneAttachment>("Character/Skeleton/BoneAttachment");
			scabbardAttachment = pawnMesh.GetNode<BoneAttachment>("Character/Skeleton/BoneAttachment2");
			//TODO: making the helmet a child of the parent for now
			helmetParent = pawnMesh.GetNode<Spatial>("Character/Skeleton/Char_Model");
			//clear the children of scabbard and node
			//There should really only be 1 child in both cases
			if(heldItemAttachment.GetChildCount() != 1 || scabbardAttachment.GetChildCount() != 1) {
					Log.Information("Child count of BoneAttachments not 1, what do?");
			}
			//loop should only run once
			foreach(Node node in heldItemAttachment.GetChildren()) {
				//I know these have to be spatials
				Spatial betterNode = (Spatial) node;
				heldItemRotation = betterNode.Rotation;
				heldItemOrigin = betterNode.Transform.origin;
				node.QueueFree();
			}
			//loop should only run once
			foreach(Node node in scabbardAttachment.GetChildren()) {
				Spatial betterNode = (Spatial) node;
				scabbardRotation = betterNode.Rotation;
				scabbardOrigin = betterNode.Transform.origin;
				node.QueueFree();
			}
		}

		public void ProcessTask(ITask task, PawnInventory pawnInventory) {
			//ok lets try this again
			if(task.Action.HeldItem == currentHeldItem) {
				//item has not changed, we dont have to do anything
				return;
			}

			//now we reset everything back to the way it was
			ClearHeldItem();
			//check to see if the currently held item should be our weapon
			//TODO: held item should be a request of item type? How would that work for potions?
			currentHeldItem = task.Action.HeldItem;
			Equipment? currentWeapon = pawnInventory.GetWornEquipment(EquipmentType.HELD);
			if(currentHeldItem != currentWeapon) {
				//then we must put our weapon back in scabbard
				PutWeaponInScabbard(currentWeapon);
			}
			//if the next item to hold is null, we are done!
			if(currentHeldItem == null) {
				return;
			}
			//new item to hold could have a parent, so we remove it
			SceneTreeUtil.OrphanChild(currentHeldItem.Mesh);
			if(currentHeldItem.Mesh.GetParent() != null) {
				currentHeldItem.Mesh.GetParent().RemoveChild(currentHeldItem.Mesh);
			}
			//now we add the new item to our hand
			heldItemAttachment.AddChild(currentHeldItem.Mesh);
			currentHeldItem.Mesh.Rotation = heldItemRotation;
			currentHeldItem.Mesh.Transform = new Transform(currentHeldItem.Mesh.Transform.basis, heldItemOrigin);
		}

		public void ForceVisualUpdate(PawnInventory pawnInventory) {
			PutWeaponInScabbard(pawnInventory.GetWornEquipment(EquipmentType.HELD));
			PutOnHelmet(pawnInventory.GetWornEquipment(EquipmentType.HEAD));
		}

		private void PutOnHelmet(Equipment? helmet) {
			SceneTreeUtil.RemoveAllChildren(helmetParent);
			if(helmet == null) {
				return;
			}
			SceneTreeUtil.OrphanChild(helmet.Mesh);
			helmetParent.AddChild(helmet.Mesh);
			helmet.Mesh.Rotation = helmetRotation;
			helmet.Mesh.Transform = new Transform(helmet.Mesh.Transform.basis, helmetOrigin);
		}

		private void ClearHeldItem() {
			if(currentHeldItem == null) {
				return;
			}
			currentHeldItem.Mesh.GetParent().RemoveChild(currentHeldItem.Mesh);
			currentHeldItem = null;
		}

		public void PutWeaponInScabbard(Equipment? currentWeapon) {
			//no weapon
			if(currentWeapon == null) {
				//make sure there is no weapon in the scabbard
				SceneTreeUtil.RemoveAllChildren(scabbardAttachment);
				return;
			}
			//if the current weapon is already in scabbard, do nothing
			if(currentWeapon.Mesh.GetParent() == scabbardAttachment) {
				return;
			}
			//if the current weapon has a parent other than the scabbard, remove it
			SceneTreeUtil.OrphanChild(currentWeapon.Mesh);
			//put weapon in scabbard
			scabbardAttachment.AddChild(currentWeapon.Mesh);
			currentWeapon.Mesh.Rotation = scabbardRotation;
			currentWeapon.Mesh.Transform = new Transform(currentWeapon.Mesh.Transform.basis, scabbardOrigin);
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