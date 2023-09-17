using Interactable;
using System;

namespace Pawn.Action.Ability;
public class AbilityBuilder
{
    readonly Ability ability;
    public AbilityBuilder(IPawnController ownerPawnController, System.Action<IInteractable?> executable, Predicate<IPawnController> canBeUsedPredicate)
    {
        ability = new Ability(ownerPawnController, executable, canBeUsedPredicate);
    }
    //todo: probably should just check cooldowns in the can be used predicate. What if I want abilites that decrease in cooldown under certain conditions?
    //the canBeUsedPredicate should check all conditions other than the cooldown of the ability. the cooldown on the ability is already handled
    public static AbilityBuilder Start(IPawnController pawnController, System.Action<IInteractable?> executable, Predicate<IPawnController> canBeUsedPredicate)
    {
        return new AbilityBuilder(pawnController, executable, canBeUsedPredicate);
    }

    public AbilityBuilder MaxRange(float range)
    {
        ability.MaxRange = range;
        return this;
    }

    public AbilityBuilder Animation(AnimationName animationName)
    {
        ability.AnimationToPlay = animationName;
        return this;
    }

    //Sets looping to be true
    public AbilityBuilder AnimationPlayLength(int milliseconds)
    {
        ability.SetAnimationPlayLength(milliseconds);
        return this;
    }

    public IAbility Finish()
    {
        return ability;
    }

    public AbilityBuilder Name(string name)
    {
        ability.Name = name;
        return this;
    }

    public AbilityBuilder CooldownMilliseconds(int milliseconds)
    {
        ability.CooldownMilliseconds = milliseconds;
        return this;
    }

}