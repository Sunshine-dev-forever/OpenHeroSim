using System.Collections.Generic;
using Godot;
using Pawn.Tasks;
using Serilog;
using Item;
using Util;

//Handles all animations and particle effects for the pawn
//TOOD: this class is very ugly but I am not sure how to refactor it
namespace Pawn
{
	public partial class PawnVisuals : Node3D
	{
		AnimationPlayer? animationPlayer = null;
		//below is all the variables needed for putting meshes in the right places
		Node3D riggedCharacterRootNode;
		BoneAttachment3D? heldItemBoneAttachment = null;
		Vector3 heldItemRotation = new Vector3();
		Vector3 heldItemOrigin = new Vector3();

		BoneAttachment3D? scabbardBoneAttachment = null;
		Vector3 scabbardRotation = new Vector3();
		Vector3 scabbardOrigin = new Vector3();
		//the pawn rig that I created does not have a helment bone attatchment
		//thus, I am just guessing where the helmet should go
		Node3D? helmetParent = null;
		Vector3 helmetOrigin = new Vector3(0,4,0);
		Vector3 helmetRotation = new Vector3();

		IItem? currentHeldItem;

		public PawnVisuals() {
			riggedCharacterRootNode = new Node3D();
		}
		public override void _Ready(){}

		//TODO: task executor will call this 
		public void UpdateHeldItem(IItem? item, IPawnInventory pawnInventory) {
			//just assume it is a different Item every single time
			ClearHeldItem();
			currentHeldItem = item;
			Equipment? currentWeapon = pawnInventory.GetWornEquipment(EquipmentType.HELD);
			if(currentHeldItem != currentWeapon) {
				//then we must put our weapon back in scabbard
				PutWeaponInScabbard(currentWeapon);
			}
			//if the next item to hold is null, we are done!
			//or if there is no place to hold the item, we are done!
			if(currentHeldItem == null || heldItemBoneAttachment == null) {
				return;
			}
			//new item to hold could have a parent, so we remove it
			SceneTreeUtil.OrphanChild(currentHeldItem.Mesh);
			//now we add the new item to our hand
			heldItemBoneAttachment.AddChild(currentHeldItem.Mesh);
			currentHeldItem.Mesh.Rotation = heldItemRotation;

			currentHeldItem.Mesh.Transform = new Transform3D(currentHeldItem.Mesh.Transform.Basis, heldItemOrigin);
		}

		public void ForceVisualUpdate(IPawnInventory pawnInventory) {
			PutWeaponInScabbard(pawnInventory.GetWornEquipment(EquipmentType.HELD));
			PutOnHelmet(pawnInventory.GetWornEquipment(EquipmentType.HEAD));
		}

		private void PutOnHelmet(Equipment? helmet) {
			if(helmetParent == null) {
				return;
			}
			SceneTreeUtil.RemoveAllChildren(helmetParent);
			if(helmet == null) {
				return;
			}
			SceneTreeUtil.OrphanChild(helmet.Mesh);
			helmetParent.AddChild(helmet.Mesh);
			helmet.Mesh.Rotation = helmetRotation;
			helmet.Mesh.Transform = new Transform3D(helmet.Mesh.Transform.Basis, helmetOrigin);
		}

		private void ClearHeldItem() {
			if(currentHeldItem == null) {
				return;
			}
			SceneTreeUtil.OrphanChild(currentHeldItem.Mesh);
			currentHeldItem = null;
		}

		private void PutWeaponInScabbard(Equipment? currentWeapon) {
			if(scabbardBoneAttachment == null) {
				return;
			}
			//no weapon
			if(currentWeapon == null) {
				//make sure there is no weapon in the scabbard
				SceneTreeUtil.RemoveAllChildren(scabbardBoneAttachment);
				return;
			}
			//if the current weapon is already in scabbard, do nothing
			if(currentWeapon.Mesh.GetParent() == scabbardBoneAttachment) {
				return;
			}
			//if the current weapon has a parent other than the scabbard, remove it
			SceneTreeUtil.OrphanChild(currentWeapon.Mesh);
			//put weapon in scabbard
			scabbardBoneAttachment.AddChild(currentWeapon.Mesh);
			currentWeapon.Mesh.Rotation = scabbardRotation;
			currentWeapon.Mesh.Transform = new Transform3D(currentWeapon.Mesh.Transform.Basis, scabbardOrigin);
		}

		public float getAnimationLengthMilliseconds(AnimationName animationName) {
			if(animationPlayer == null) {
				//so basically anything with a missing animation gets to instantly use all of its abilities
				//TODO: is there a better solution than this?
				//1 millisecond should not break anything, 0 might
				return 1;
			}
			//TODO: find some constants library that defines this
			int MILLISECONDS_IN_SECOND = 1000;
			return animationPlayer.GetAnimation(animationName.ToString()).Length * MILLISECONDS_IN_SECOND;
		}

		//TODO: this should take in a LoopModeEnum
		public void SetAnimation(AnimationName animationName, bool looping = false) {
			if(animationPlayer == null) {
				return;
			}
			if(looping) {
				animationPlayer.GetAnimation(animationName.ToString()).LoopMode = Animation.LoopModeEnum.Linear;
			}
			animationPlayer.Play(animationName.ToString());
		}

		public void setPawnRotation(float yRotation) {
			//Only the y rotaiton should ever be changed
			riggedCharacterRootNode.Rotation = new Vector3(0,yRotation,0);
		}

		//I really need a pawn rig loader
		//TODO: SetPawnRig and all it's related functions should be in its own class called "PawnRigLoader"
		public void SetPawnRig(string filename) {
			Node3D pawnMesh = CustomResourceLoader.LoadMesh(filename);
			//clear all old nodes
			foreach (Node node in this.GetChildren()) {
				node.QueueFree();
			}
			this.AddChild(pawnMesh);
			riggedCharacterRootNode = pawnMesh;
			//TODO need a better way of extracting this information
			animationPlayer = pawnMesh.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
			SetupHeldItem(pawnMesh);
			SetupHelmet(pawnMesh);
			SetupScabbard(pawnMesh);
		}

		//TODO: SetupHeldItem, SetupScabbard, SetupHelmet, and whatever else should be in a for loop some how
		private void SetupHeldItem(Node3D pawnMeshRoot) {
			heldItemBoneAttachment = pawnMeshRoot.GetNodeOrNull<BoneAttachment3D>("Character/Skeleton3D/Held_Item");
			if(heldItemBoneAttachment == null) {
				//should be common, silently fail
				return;
			}
			if(heldItemBoneAttachment.GetChildCount() > 1) {
				Log.Warning("More than one child found in setupHeldItem");
			}
			//loop should only run once
			//Clears children
			foreach(Node node in heldItemBoneAttachment.GetChildren()) {
				//I know these have to be spatials
				Node3D betterNode = (Node3D) node;
				heldItemRotation = betterNode.Rotation;
				heldItemOrigin = betterNode.Transform.Origin;
				node.QueueFree();
			}
		}

		private void SetupScabbard(Node3D pawnMeshRoot) {
			scabbardBoneAttachment = pawnMeshRoot.GetNodeOrNull<BoneAttachment3D>("Character/Skeleton3D/Scabbard");
			if(scabbardBoneAttachment == null) {
				//should be common, silently fail
				return;
			}
			if(scabbardBoneAttachment.GetChildCount() > 1) {
				Log.Warning("More than one child found in SetupScabbard");
			}
			//loop should only run once
			foreach(Node node in scabbardBoneAttachment.GetChildren()) {
				Node3D betterNode = (Node3D) node;
				scabbardRotation = betterNode.Rotation;
				scabbardOrigin = betterNode.Transform.Origin;
				node.QueueFree();
			}
		}

		private void SetupHelmet(Node3D pawnMeshRoot) {
			//TODO: making the helmet a child of the whole Char_model for now (Char_Model should have no other children)
			helmetParent = pawnMeshRoot.GetNodeOrNull<Node3D>("Character/Skeleton3D/Char_Model");
			if(helmetParent == null) {
				return;
			}
			//there should be no children no matter what
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