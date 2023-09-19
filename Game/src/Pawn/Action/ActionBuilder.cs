namespace Pawn.Action;
public class ActionBuilder
{
    readonly Action action;

    ActionBuilder(IPawnController ownerPawnController, System.Action executable)
    {
        action = new Action(ownerPawnController, executable);
    }
    public static ActionBuilder Start(IPawnController ownerPawnController, System.Action executable)
    {
        return new ActionBuilder(ownerPawnController, executable);
    }

    public ActionBuilder MaxRange(float range)
    {
        action.MaxRange = range;
        return this;
    }

    public ActionBuilder Animation(AnimationName animationName)
    {
        action.AnimationToPlay = animationName;
        return this;
    }

    //Sets looping to be true
    public ActionBuilder AnimationPlayLength(int milliseconds)
    {
        action.SetAnimationPlayLength(milliseconds);
        return this;
    }

    public IAction Finish()
    {
        return action;
    }

    public ActionBuilder Name(string name)
    {
        action.Name = name;
        return this;
    }
}