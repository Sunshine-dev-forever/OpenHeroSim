using Godot;
using Serilog;
using System;

namespace Pawn.Action;

// actions are one-off things a pawn can do. They are used once and thrown away
// actions have no cooldown (they cant have one)
// examples of actions: buy something from a shopkeeper, consume food or drink. Talk to someone
public class Action : IAction {
    static readonly int NO_COOLDOWN = 0;
    static readonly float DEFAULT_RANGE = 2;
    static readonly Animation.LoopModeEnum NO_LOOPING = Animation.LoopModeEnum.None;

    readonly IPawnController ownerPawnController;

    bool executableHasBeenRun = false;
    public Animation.LoopModeEnum LoopMode { get; set; } = NO_LOOPING;
    public int CooldownMilliseconds { get; set; } = NO_COOLDOWN;
    public string Name { get; set; } = "Generic Action";
    public float MaxRange { get; set; } = DEFAULT_RANGE;

    bool isCurrentlyRunning = false;
    DateTime timeStarted = DateTime.MinValue;
    // the function that makes the Action actually do something in the game world
    public System.Action Executable { get; set; }
    public AnimationName AnimationToPlay { get; set; } = AnimationName.Interact;

    double loopingAnimationPlayLength = -1;

    double AnimationPlayLengthMilliseconds {
        get {
            if (LoopMode == Animation.LoopModeEnum.None) {
                return ownerPawnController.PawnVisuals
                    .GetAnimationLengthMilliseconds(AnimationToPlay);
            }
            else {
                // we are looping, so looping animation play length must be set so some useful value
                return loopingAnimationPlayLength;
            }
        }
    }

    public Action(IPawnController _ownerPawnController, System.Action _executable) {
        ownerPawnController = _ownerPawnController;
        Executable = _executable;
    }

    // sets looping to true
    public void SetAnimationPlayLength(double milliseconds) {
        loopingAnimationPlayLength = milliseconds;
        LoopMode = Animation.LoopModeEnum.Linear;
    }

    public void Start() {
        if (isCurrentlyRunning) {
            Log.Error("Attempted to start the same action twice");
            Log.Error(System.Environment.StackTrace);
            throw new InvalidOperationException();
        }

        ownerPawnController.PawnVisuals.SetAnimation(AnimationToPlay, LoopMode);
        isCurrentlyRunning = true;
        timeStarted = DateTime.Now;
    }

    public bool IsFinished() {
        if (!isCurrentlyRunning) {
            throw new InvalidOperationException();
        }

        if (!executableHasBeenRun) {
            return false;
        }

        double timeRunningMilliseconds =
            (DateTime.Now - timeStarted).TotalMilliseconds;

        return timeRunningMilliseconds > AnimationPlayLengthMilliseconds;
    }

    public void Process() {
        if (!isCurrentlyRunning) {
            throw new InvalidOperationException();
        }

        // only gets milliseconds between 0 and 1000
        double timeRunningMilliseconds =
            (DateTime.Now - timeStarted).TotalMilliseconds;

        const double ONE_HALF = 1.0 / 2.0;

        // there is no looping, so we can just use the original animation length
        if (timeRunningMilliseconds > (AnimationPlayLengthMilliseconds * ONE_HALF) && !executableHasBeenRun) {
            Executable();
            executableHasBeenRun = true;
        }
    }
}
