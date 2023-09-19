using Godot;
using Util;

// Handles all animations and particle effects for the pawn
// Needs to have the Setupfunction called before this node will function
namespace Pawn;

public partial class PawnVisuals : Node3D
{
    const AnimationName DEFAULT_ANIMATION = AnimationName.Interact;
    AnimationPlayer? animationPlayer = null;
    Node3D riggedCharacterRootNode = null!;

    public float getAnimationLengthMilliseconds(AnimationName animationName)
    {
        if (animationPlayer == null || animationPlayer.GetAnimation(GetAnimationNameOrDefault(animationName)) == null)
        {
            // so basically anything with a missing animation gets to instantly use all of its abilities
            // TODO: is there a better solution than this?
            return 0;
        }

        // TODO: find some constants library that defines this
        int MILLISECONDS_IN_SECOND = 1000;
        return animationPlayer.GetAnimation(GetAnimationNameOrDefault(animationName)).Length * MILLISECONDS_IN_SECOND;
    }

    public void SetAnimation(AnimationName animationName, Animation.LoopModeEnum loopMode = Animation.LoopModeEnum.None)
    {
        if (animationPlayer == null)
        {
            return;
        }

        animationPlayer.GetAnimation(GetAnimationNameOrDefault(animationName)).LoopMode = loopMode;
        animationPlayer.Play(GetAnimationNameOrDefault(animationName));
    }

    // returns the animation name converted to a string, or the default animation name if the current rig does not support 
    // the requested animation
    // TODO: if the current animation player is null or also does not support the default animatoin, throw an error
    string GetAnimationNameOrDefault(AnimationName animationName)
    {
        // if animation player is null that is a major issue, but this function will just return the default
        return animationPlayer == null
            ? DEFAULT_ANIMATION.ToString()
            : animationPlayer.HasAnimation(animationName.ToString()) ? animationName.ToString() : DEFAULT_ANIMATION.ToString();
    }

    public void setPawnRotation(float yRotation)
    {
        // Only the y rotaiton should ever be changed
        riggedCharacterRootNode.Rotation = new Vector3(0, yRotation, 0);
    }

    // I really need a pawn rig loader
    // TODO: SetPawnRig and all it's related functions should be in its own class called "PawnRigLoader"
    public void Setup(string filenamePawnMesh)
    {
        Node3D pawnMesh = CustomResourceLoader.LoadMesh(filenamePawnMesh);
        
        // clear all old nodes
        foreach (Node node in this.GetChildren())
        {
            node.QueueFree();
        }

        this.AddChild(pawnMesh);
        riggedCharacterRootNode = pawnMesh;
        animationPlayer = pawnMesh.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
    }
}

public enum AnimationName
{
    Die,
    Consume,
    Idle,
    Interact,
    Walk,
    MeleeAttack,
    RangedAttack
}
