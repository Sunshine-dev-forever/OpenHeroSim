using Item;
using Pawn.Action;
using Pawn.Components;
using Pawn.Targeting;
using Pawn.Tasks;
namespace Pawn.Goal;

public class HealGoal : IPawnGoal
{
    public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct)
    {
        IItem? currentItem = null;
        foreach (IItem item in pawnController.PawnInventory.GetAllItemsInBag())
        {
            if (item is Consumable)
            {
                currentItem = item;
                break;
            }
        }

        if (currentItem == null)
        {
            //if we have no consumables, then we early exit
            return new InvalidTask();
        }

        if (pawnController.PawnInformation.Health > 50)
        {
            //we are not hurt, no reason to use a potion
            return new InvalidTask();
        }

        Consumable potion = (Consumable)currentItem;
        void executable()
        {
            pawnController.PawnInventory.RemoveItem(potion);
            //TODO: TakeDamage should be called 'change health'
            pawnController.TakeHealing(potion.Healing);
        }
        //we know it is only health potions
        IAction action = ActionBuilder.Start(pawnController, executable)
                                        .Animation(AnimationName.Consume)
                                        .Finish();
        ITargeting targeting = new InteractableTargeting(pawnController);
        return new Task(targeting, action, "Healing self with a consumable");
    }
}
