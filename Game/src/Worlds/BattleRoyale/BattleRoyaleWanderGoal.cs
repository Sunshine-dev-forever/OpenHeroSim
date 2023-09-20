using Godot;
using Pawn;
using Pawn.Action;
using Pawn.Components;
using Pawn.Goal;
using Pawn.Targeting;
using Pawn.Tasks;
using System;

namespace Worlds.BattleRoyale;

public class BattleRoyaleWanderGoal : IPawnGoal
{
    public BattleRoyaleWanderGoal() { }

    public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct)
    {
        int sideLength = (int) (FogController.GetFogController().GetFogPosition() * 2);
        
        Random random = new();
        
        float x = (float) ((random.NextDouble() * sideLength) - (sideLength/2));
        float z = (float) ((random.NextDouble() * sideLength) - (sideLength/2));
        
        int waitTimeMilliseconds = 2000;
        
        IAction action = ActionBuilder.Start(pawnController, () => {})
            .Animation(AnimationName.Idle)
            .AnimationPlayLength(waitTimeMilliseconds)
            .Finish();

        Predicate<Vector3> predicate = FogController.GetFogController().IsInbounds;
        
        ITargeting targeting = new StaticPointTargeting(new Vector3(x,5,z), predicate);
        
        return new Task(targeting, action);
    }
}
