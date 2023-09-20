using Pawn.Action;
using Pawn.Components;
using Pawn.Targeting;
using Pawn.Tasks;

namespace Pawn.Goal;

public class DebugGoal : IPawnGoal
{
    public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct)
    {
        int waitTimeMilliseconds = 10000;

        IAction waitAction = ActionBuilder
            .Start(pawnController, () => { })
            .Animation(AnimationName.Idle)
            .AnimationPlayLength(waitTimeMilliseconds)
            .Finish();

        // we have the pawn target itself
        ITargeting targeting = new InteractableTargeting(pawnController);

        return new Task(
            targeting, 
            waitAction, 
            "Waiting in place for ever and ever (DebugGoal)");
    }
}
