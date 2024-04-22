using Pawn.Action;
using Pawn.Components;
using Pawn.Targeting;
using Pawn.Tasks;
using System;

namespace Pawn.Goal;

public class WanderGoal : IPawnGoal {
    // center is always at (0,0)
    readonly int sideLength;

    public WanderGoal() {
        sideLength = 50;
    }

    public WanderGoal(int _sideLength) {
        sideLength = _sideLength;
    }

    public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct) {
        Random random = new();
        float x = (float)((random.NextDouble() * sideLength) - (sideLength / 2));
        float z = (float)((random.NextDouble() * sideLength) - (sideLength / 2));
        int waitTimeMilliseconds = 2000;
        IAction action = ActionBuilder
            .Start(pawnController, () => { })
            .Animation(AnimationName.Idle)
            .AnimationPlayLength(waitTimeMilliseconds)
            .Finish();

        ITargeting targeting = new StaticPointTargeting(new Godot.Vector3(x, 5, z));
        return new Task(targeting, action, "wandering about");
    }
}
