using System.Collections.Generic;
using Godot;
using Pawn.Tasks;
using Serilog;
using Item;
using Util;

//Handles all animations and particle effects for the pawn
//Needs to have the Setupfunction called before this node will function
namespace Pawn
{
	public partial class PawnVisuals : Node3D
	{
		private const AnimationName DEFAULT_ANIMATION = AnimationName.Interact;
		private AnimationPlayer? animationPlayer = null;
		private Node3D riggedCharacterRootNode = null!;
		public float getAnimationLengthMilliseconds(AnimationName animationName) {
			if(animationPlayer == null || animationPlayer.GetAnimation(animationName.ToString()) == null) {
				//so basically anything with a missing animation gets to instantly use all of its abilities
				//TODO: is there a better solution than this?
				//1 millisecond should not break anything, 0 might
				return 1;
			}
			//TODO: find some constants library that defines this
			int MILLISECONDS_IN_SECOND = 1000;
			return animationPlayer.GetAnimation(animationName.ToString()).Length * MILLISECONDS_IN_SECOND;
		}

		public void SetAnimation(AnimationName animationName, Animation.LoopModeEnum loopMode = Animation.LoopModeEnum.None) {
			if(animationPlayer == null) {
				return;
			}
			//TODO: something about the few lines below might not work right
			if(animationPlayer.HasAnimation(animationName.ToString())) {
				animationPlayer.GetAnimation(animationName.ToString()).LoopMode = loopMode;
				animationPlayer.Play(animationName.ToString());
			} else {
				animationPlayer.GetAnimation(DEFAULT_ANIMATION.ToString()).LoopMode = loopMode;
				animationPlayer.Play(DEFAULT_ANIMATION.ToString());
			}
			

		}

		public void setPawnRotation(float yRotation) {
			//Only the y rotaiton should ever be changed
			riggedCharacterRootNode.Rotation = new Vector3(0,yRotation,0);
		}

		//I really need a pawn rig loader
		//TODO: SetPawnRig and all it's related functions should be in its own class called "PawnRigLoader"
		public void Setup(string filenamePawnMesh) {
			Node3D pawnMesh = CustomResourceLoader.LoadMesh(filenamePawnMesh);
			//clear all old nodes
			foreach (Node node in this.GetChildren()) {
				node.QueueFree();
			}
			this.AddChild(pawnMesh);
			riggedCharacterRootNode = pawnMesh;
			animationPlayer = pawnMesh.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		}
	}

	public enum AnimationName {
		Death,
		Drink,
		Idle,
		Interact,
		Walking,
		Stab,
		Shoot
	}
}