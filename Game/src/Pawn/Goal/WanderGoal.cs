using Pawn.Action;
using Pawn.Components;
using Pawn.Targeting;
using Pawn.Tasks;
using System;
using Godot;

namespace Pawn.Goal;

public class WanderGoal : IPawnGoal {

    readonly Func<Vector3> destinationGetter;
    readonly int waitTimeMilliseconds = 2000;

    public WanderGoal(Func<Vector3> _destinationGetter) {
        destinationGetter = _destinationGetter;
    }

    public WanderGoal(int _waitTimeMilliseconds, Func<Vector3> _destinationGetter) {
        waitTimeMilliseconds = _waitTimeMilliseconds;
        destinationGetter = _destinationGetter;
    }

    public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct) {
        IAction action = ActionBuilder
            .Start(pawnController, () => { })
            .Animation(AnimationName.Idle)
            .AnimationPlayLength(waitTimeMilliseconds)
            .Finish();

        ITargeting targeting = new StaticPointTargeting(destinationGetter());
        return new Task(targeting, action, "wandering about");
    }
}
