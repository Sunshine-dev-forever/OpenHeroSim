namespace Pawn.Action;
public interface IAction {
    // name of the action
    string Name { get; }
    // the maximum range at which the action can be started, note that the action may not finish at this range
    float MaxRange { get; }
    // begins the action
    void Start();
    // returns true if the action has completed, otherwise false
    bool IsFinished();
    // computes things that an action needs
    // not sure how to describe this, should be called every frame to allow the action to process things
    // that it might need to compute, but the action class also understands that this does not have to be called every
    // frame,
    // now I am just rambling
    void Process();
}
