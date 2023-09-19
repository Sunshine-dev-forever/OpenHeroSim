using Godot;
using Interactable;
using Serilog;
using System;

namespace Pawn.Action.Ability;
//abilities may be used more than once, and maintain data between uses
//abilities have cooldowns
//examples of abilities would be: shoot fire ball!, super-uber-death stab. ETC
public class Ability : IAbility
{
    static readonly int DEFAULT_COOLDOWN_MILLISECONDS = 2000;
    static readonly float DEFAULT_RANGE = 2;
    static readonly Animation.LoopModeEnum DEFAULT_LOOPING = Animation.LoopModeEnum.None;
    bool hasAbilityExecutableBeenRun = false;

    System.Action<IInteractable?> abilityExecutable { get; set; }
    Predicate<IPawnController> canBeUsedPredicate { get; set; }

    IInteractable? target;
    public AnimationName AnimationToPlay { get; set; } = AnimationName.Interact;
    public Animation.LoopModeEnum loopMode { get; set; } = DEFAULT_LOOPING;

    readonly IPawnController ownerPawnController;
    public int CooldownMilliseconds { get; set; } = DEFAULT_COOLDOWN_MILLISECONDS;
    public string Name { get; set; } = "Generic ability";
    public float MaxRange { get; set; } = DEFAULT_RANGE;

    bool isCurrentlyRunning = false;
    DateTime timeStarted = DateTime.MinValue;
    DateTime lastTimeAbilityUsed = DateTime.MinValue;
    double loopingAnimationPlayLength = -1;

    double AnimationPlayLengthMilliseconds
    {
        get
        {
            if (loopMode == Animation.LoopModeEnum.None)
            {
                return ownerPawnController.PawnVisuals.getAnimationLengthMilliseconds(AnimationToPlay);
            }
            else
            {
                //we are looping, so looping animation play length must be set so some useful value
                return loopingAnimationPlayLength;
            }
        }
    }

    public Ability(IPawnController _ownerPawnController, System.Action<IInteractable?> _executable, Predicate<IPawnController> _canBeUsedPredicate)
    {
        ownerPawnController = _ownerPawnController;
        abilityExecutable = _executable;
        canBeUsedPredicate = _canBeUsedPredicate;
    }
    //I only set a animation play length for looping animations, so this sets looping to be true
    public void SetAnimationPlayLength(int milliseconds)
    {
        loopingAnimationPlayLength = milliseconds;
        loopMode = Animation.LoopModeEnum.Linear;
    }

    public void Start()
    {
        if (isCurrentlyRunning)
        {
            Log.Error("Attempted to start the same ability twice: " + Name);
            Log.Error(System.Environment.StackTrace);
        }

        lastTimeAbilityUsed = DateTime.Now;
        ownerPawnController.PawnVisuals.SetAnimation(AnimationToPlay, loopMode);
        isCurrentlyRunning = true;
        timeStarted = DateTime.Now;
    }
    public bool IsFinished()
    {
        if (!isCurrentlyRunning)
        {
            throw new InvalidOperationException();
        }

        if (!hasAbilityExecutableBeenRun)
        {
            return false;
        }

        double timeRunningMilliseconds = (DateTime.Now - timeStarted).TotalMilliseconds;

        return timeRunningMilliseconds > AnimationPlayLengthMilliseconds;
    }

    void ResetAbility()
    {
        isCurrentlyRunning = false;
        hasAbilityExecutableBeenRun = false;
    }

    public bool CanBeUsed(IPawnController ownerPawnController)
    {
        bool offCoolDown = (DateTime.Now - lastTimeAbilityUsed).TotalMilliseconds > CooldownMilliseconds;
        bool ret = offCoolDown && canBeUsedPredicate(ownerPawnController);
        return ret;
    }

    public void Setup(IInteractable? _target)
    {
        ResetAbility();
        target = _target;
    }

    public void Process()
    {
        if (!isCurrentlyRunning)
        {
            throw new InvalidOperationException();
        }

        const double ONE_HALF = 1.0 / 2.0;
        double timeRunningMilliseconds = (DateTime.Now - timeStarted).TotalMilliseconds;
        if (timeRunningMilliseconds > AnimationPlayLengthMilliseconds * ONE_HALF && !hasAbilityExecutableBeenRun)
        {
            abilityExecutable(target);
            hasAbilityExecutableBeenRun = true;
        }
    }
}
