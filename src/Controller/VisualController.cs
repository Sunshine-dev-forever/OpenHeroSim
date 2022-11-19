using System.Collections.Generic;
using Godot;

//Handles all animations and particle effects.. etc
public class VisualController : Spatial
{
	public AnimationPlayer GetAnimationPlayer() {
		return this.GetNode<AnimationPlayer>("RiggedCharacter/AnimationPlayer");
	}

	public Spatial GetRiggedCharacterRootNode() {
		return this.GetNode<Spatial>("RiggedCharacter");
	}
}