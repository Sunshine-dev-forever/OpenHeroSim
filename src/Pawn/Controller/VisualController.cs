using System.Collections.Generic;
using Godot;
using Pawn.Tasks;

//Handles all animations and particle effects.. etc
namespace Pawn.Controller
{
	public class VisualController : Spatial
	{
		AnimationPlayer animationPlayer = null!;
		Spatial riggedCharacterRootNode = null!;

		MeshInstance scabbard = null!;
		MeshInstance heldItem = null!;

		public override void _Ready()
		{
			//TODO need a better way of extracting this information
			animationPlayer = this.GetNode<AnimationPlayer>("RiggedCharacter/AnimationPlayer");
			riggedCharacterRootNode = this.GetNode<Spatial>("RiggedCharacter");
			heldItem = this.GetNode<MeshInstance>("RiggedCharacter/Character/Skeleton/BoneAttachment/Held_Item");
			scabbard = this.GetNode<MeshInstance>("RiggedCharacter/Character/Skeleton/BoneAttachment2/Scabbard");
		}

		public void ProcessTask(ITask task) {
			if(task.isCombat) {
				scabbard.Visible = false;
				heldItem.Visible = true;
			} else {
				scabbard.Visible = true;
				heldItem.Visible = false;
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